using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using LibreSplit.Timing;
using System.IO;
using System.Threading.Tasks;
using LibreSplit.IO;
using LibreSplit.Layouts;
using Avalonia.Platform;

namespace LibreSplit.UI.Windows;

public class MainWindowViewModel {
  IStorageFolder? defaultStartLocation;
  private static FilePickerFileType SplitsFileType { get; } = new("Splits File") {
    Patterns = ["*.lbss"],
    AppleUniformTypeIdentifiers = [],
    MimeTypes = []
  };
  private static FilePickerFileType LayoutFileType { get; } = new("LibreSplit Layout File") {
    Patterns = ["*.lbsl"],
    AppleUniformTypeIdentifiers = [],
    MimeTypes = []
  };
  private FilePickerOpenOptions splitsOpenOptions = new() {
    AllowMultiple = false,
    FileTypeFilter = [SplitsFileType]
  };
  private FilePickerSaveOptions splitsSaveOptions = new() {
    SuggestedFileName = "New Splits",
    FileTypeChoices = [SplitsFileType],
  };
  private FilePickerOpenOptions layoutOpenOptions = new() {
    AllowMultiple = false,
    FileTypeFilter = [LayoutFileType],
  };
  private FilePickerSaveOptions layoutSaveOptions = new() {
    SuggestedFileName = "New LibreSplit Layout",
    FileTypeChoices = [LayoutFileType],
  };

  private string? loadedSplitsFile;
  private string? loadedLayoutFile;
  private readonly ConfigLoader configLoader = new();
  private readonly Serializer serializer = new();
  public static LibreSplitContext GlobalContext { get; set; } = new();

  private IStorageProvider StorageProvider { get; set; }

  public MainWindowViewModel(IStorageProvider storageProvider) {
    this.StorageProvider = storageProvider;
    Task.Run(async () => {
      defaultStartLocation = await storageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents);

      var splitsStartLocation = defaultStartLocation;
      if (loadedSplitsFile is not null && Path.GetDirectoryName(loadedSplitsFile) is string dir) {
        splitsStartLocation = await storageProvider.TryGetFolderFromPathAsync(dir);
      }
      var layoutStartLocation = defaultStartLocation;
      if (loadedLayoutFile is not null && Path.GetDirectoryName(loadedLayoutFile) is string layoutDir) {
        layoutStartLocation = await storageProvider.TryGetFolderFromPathAsync(layoutDir);
      }

      splitsOpenOptions.SuggestedStartLocation = splitsStartLocation;
      splitsSaveOptions.SuggestedStartLocation = splitsStartLocation;

      layoutOpenOptions.SuggestedStartLocation = layoutStartLocation;
      layoutSaveOptions.SuggestedStartLocation = layoutStartLocation;
    });



    configLoader.LoadOrCreate();
    configLoader.TryLoadSplits(out loadedSplitsFile);

    if (loadedSplitsFile != null && serializer.Read<RunData>(loadedSplitsFile, out var run)) {
      GlobalContext.Run = run;
    }
    else {
      configLoader.Set(ConfigKeys.LastLoadedSplits, "");
    }

    configLoader.TryLoadLayout(out loadedLayoutFile);

    if (loadedLayoutFile != null && serializer.Read<LayoutData>(loadedLayoutFile, out var layout)) {
      GlobalContext.LayoutData.Items.Clear();
      foreach (var layoutItem in layout.Items) {
        GlobalContext.LayoutData.Items.Add(layoutItem);
      }
    }
    else {
      GlobalContext.LayoutData = new();
      configLoader.Set(ConfigKeys.LastLoadedLayout, "");
    }

    GlobalContext.Initialize();
    GlobalContext.InitializeInputAndKeymap(configLoader);
  }

  internal async void OpenSplits() {
    if (GlobalContext.Timer.Running) {
      YesNoCancel.Result result = await YesNoCancel.Window.Open(new YesNoCancel.ViewModel() {
        Prompt = "This run is currently in progress. Are you sure you want to open different splits?",
      });

      if (result.HasFlag(YesNoCancel.Result.NoOrCancel)) {
        return;
      }

      GlobalContext.Timer.Stop();
    }

    if (GlobalContext.ActiveSegment is not null) {
      GlobalContext.ActiveSegment = null;
    }

    if (loadedSplitsFile is not null && Path.GetDirectoryName(loadedSplitsFile) is string dir) {
      splitsOpenOptions.SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(dir);
    }
    var list = await StorageProvider.OpenFilePickerAsync(splitsOpenOptions);
    if (list.Count == 0) {
      return;
    }
    loadedSplitsFile = list[0].Path?.AbsolutePath!;

    if (loadedSplitsFile != null && serializer.Read<RunData>(loadedSplitsFile, out var run)) {
      configLoader.Set(ConfigKeys.LastLoadedSplits, loadedSplitsFile);
      GlobalContext.Run = run;
    }
    else {
      configLoader.Set(ConfigKeys.LastLoadedSplits, "");
    }
  }

  internal static async void NewSplits() {
    if (GlobalContext.Timer.Running) {
      YesNoCancel.Result result = await YesNoCancel.Window.Open(new YesNoCancel.ViewModel() {
        Prompt = "This run is currently in progress. Are you sure you want to open new splits?",
      });
      if (result.HasFlag(YesNoCancel.Result.NoOrCancel)) {
        return;
      }
    }

    if (GlobalContext.ActiveSegment is not null) {
      GlobalContext.ActiveSegment = null;
    }

    GlobalContext.Run = new();
  }

  internal void SaveSplits() {
    if (GlobalContext.Run == null) {
      Logs.LogError("Run was null when tried to save.");
      return;
    }
    if (loadedSplitsFile == null) {
      SaveSplitsAs();
      return;
    }
    if (serializer.Write(loadedSplitsFile, GlobalContext.Run)) {
      configLoader.Set(ConfigKeys.LastLoadedSplits, loadedSplitsFile);
    }
  }

  internal async void SaveSplitsAs() {
    if (GlobalContext.Run == null) {
      Logs.LogError("Run was null when tried to save.");
      return;
    }
    if (loadedSplitsFile is not null && Path.GetDirectoryName(loadedSplitsFile) is string dir) {
      splitsSaveOptions.SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(dir);
    }
    var file = await StorageProvider.SaveFilePickerAsync(splitsSaveOptions);
    if (file == null) {
      Logs.LogInfo("You must select a file to 'Save As'");
      return;
    }
    if (serializer.Write(file.Path.AbsolutePath, GlobalContext.Run)) {
      loadedSplitsFile = file.Path.AbsolutePath;
      configLoader.Set(ConfigKeys.LastLoadedSplits, loadedSplitsFile);
    }
  }

  internal static async void EditSplits(Window ownerWindow) {
    if (GlobalContext.Run == null) {
      NewSplits();
    }
    var window = new RunEditor(GlobalContext.Run);
    GlobalContext.StartEditing();
    await window.ShowDialog(owner: ownerWindow);
    GlobalContext.StopEditing();
    GlobalContext.Run = window.GetRun() ?? new();
  }

  internal async void EditKeybinds(Window ownerWindow) {
    GlobalContext.StartEditing();
    var win = new KeybindsEditor(GlobalContext);
    await win.ShowDialog(owner: ownerWindow);
    GlobalContext.StopEditing();
    configLoader.Set("keymap", GlobalContext.keymap);
  }

  internal static void NewLayout() {
    GlobalContext.LayoutData = new();
  }

  internal async void OpenLayout() {
    if (loadedLayoutFile is not null && Path.GetDirectoryName(loadedLayoutFile) is string dir) {
      layoutOpenOptions.SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(dir);
    }
    var list = await StorageProvider.OpenFilePickerAsync(layoutOpenOptions);
    if (list.Count == 0) {
      return;
    }
    loadedLayoutFile = list[0].Path?.AbsolutePath!;

    if (loadedLayoutFile != null && serializer.Read<LayoutData>(loadedLayoutFile, out var layout)) {
      configLoader.Set(ConfigKeys.LastLoadedLayout, loadedLayoutFile);
      GlobalContext.LayoutData.Items.Clear();
      foreach (var layoutItem in layout.Items) {
        GlobalContext.LayoutData.Items.Add(layoutItem);
      }
    }
    else {
      configLoader.Set(ConfigKeys.LastLoadedLayout, "");
    }
  }

  internal void SaveLayout() {
    if (loadedLayoutFile == null) {
      SaveLayoutAs();
      return;
    }
    if (serializer.Write(loadedLayoutFile, GlobalContext.LayoutData.Items)) {
      configLoader.Set(ConfigKeys.LastLoadedLayout, loadedLayoutFile);
    }
  }

  internal async void SaveLayoutAs() {
    if (loadedLayoutFile is not null && Path.GetDirectoryName(loadedLayoutFile) is string dir) {
      layoutSaveOptions.SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(dir);
    }
    var file = await StorageProvider.SaveFilePickerAsync(layoutSaveOptions);
    if (file == null) {
      Logs.LogInfo("You must select a file to 'Save As'");
      return;
    }
    var path = file.Path.AbsolutePath;
    if (serializer.Write(path, GlobalContext.LayoutData.Items)) {
      loadedLayoutFile = path;
      configLoader.Set(ConfigKeys.LastLoadedLayout, loadedLayoutFile);
    }
  }

  public static async void EditLayout(Window ownerWindow) {
    var window = new LayoutEditor();
    await window.ShowDialog(owner: ownerWindow);
  }


  /// <summary>
  /// </summary>
  /// <param name="e"></param>
  /// <returns>
  /// true if there are no unsaved changes, or the changes were saved here.
  /// false if there are unsaved changes, and the user does not want to proceed with whatever operation is requesting this.
  /// </returns>
  internal async Task<bool> CheckForUnsavedChangesInSplits() {
    if (loadedSplitsFile is null || serializer.HasUnsavedChanges(loadedSplitsFile, GlobalContext.Run)) {
      var result = await YesNoCancel.Window.Open(new YesNoCancel.ViewModel() {
        Prompt = "You have unsaved changes in your splits. Do you want to save before closing?",
      });
      if (result.HasFlag(YesNoCancel.Result.Yes)) {
        SaveSplits();
      }
      else if (result.HasFlag(YesNoCancel.Result.Cancel)) {
        return false;
      }
    }
    return true;
  }
  /// <summary>
  /// </summary>
  /// <param name="e"></param>
  /// <returns>
  /// true if there are no unsaved changes, or the changes were saved here.
  /// false if there are unsaved changes, and the user does not want to proceed with whatever operation is requesting this.
  /// </returns>
  internal async Task<bool> CheckForUnsavedChangesInLayout() {
    if (loadedLayoutFile is not null && serializer.HasUnsavedChanges(loadedLayoutFile, GlobalContext.LayoutData)) {
      var result = await YesNoCancel.Window.Open(new YesNoCancel.ViewModel() {
        Prompt = "You have unsaved changes in your layout. Do you want to save before closing?",
      });
      if (result.HasFlag(YesNoCancel.Result.Yes)) {
        SaveLayout();
      }
      else if (result.HasFlag(YesNoCancel.Result.Cancel)) {
        return false;
      }
    }
    return true;
  }
}

public partial class MainWindow : Window {
  private readonly MainWindowViewModel m_viewModel;
  public static LibreSplitContext GlobalContext => MainWindowViewModel.GlobalContext;
  
  public MainWindow() {
    m_viewModel = new(StorageProvider);
    Closing += OnClosing;
    DataContext = GlobalContext;
    InitializeComponent();
    Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://LibreSplit/Assets/icon.png")));
    Topmost = true;
  }
  private void OnClosing(object? sender, WindowClosingEventArgs e) {
    Input.Stop();
  }
  public void NewSplitsClicked(object sender, RoutedEventArgs _) {
    MainWindowViewModel.NewSplits();
  }
  public void OpenSplitsClicked(object sender, RoutedEventArgs _) {
    m_viewModel.OpenSplits();
  }
  public void SaveSplitsClicked(object sender, RoutedEventArgs _) {
    m_viewModel.SaveSplits();
  }
  public void SaveSplitsAsClicked(object sender, RoutedEventArgs _) {
    m_viewModel.SaveSplitsAs();
  }
  public void EditSplitsClicked(object sender, RoutedEventArgs _) {
    MainWindowViewModel.EditSplits(this);
  }
  public void EditKeybindsClicked(object sender, RoutedEventArgs _) {
    m_viewModel.EditKeybinds(this);
  }
  public void NewLayoutClicked(object sender, RoutedEventArgs _) {
    MainWindowViewModel.NewLayout();
  }
  public void OpenLayoutClicked(object sender, RoutedEventArgs _) {
    m_viewModel.OpenLayout();
  }
  public void SaveLayoutClicked(object sender, RoutedEventArgs _) {
    m_viewModel.SaveLayout();
  }
  public void SaveLayoutAsClicked(object sender, RoutedEventArgs _) {
    m_viewModel.SaveLayoutAs();
  }
  public void EditLayoutClicked(object sender, RoutedEventArgs _) {
    MainWindowViewModel.EditLayout(this);
  }
  public async void CloseWindowClicked(object sender, RoutedEventArgs _) {
    if (!await m_viewModel.CheckForUnsavedChangesInSplits()) {
      return;
    }
    if (!await m_viewModel.CheckForUnsavedChangesInLayout()) {
      return;
    }
    Close();
  }
}
