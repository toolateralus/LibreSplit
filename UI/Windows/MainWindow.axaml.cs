using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using LibreSplit.Timing;
using System.IO;
using System.Threading.Tasks;
using LibreSplit.IO;
using LibreSplit.Layouts;
using Avalonia.Logging;

namespace LibreSplit.UI.Windows;

public partial class MainWindow : Window {
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
  public MainWindow() {
    Task.Run(async () => {
      defaultStartLocation = await StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents);

      var splitsStartLocation = defaultStartLocation;
      if (loadedSplitsFile is not null && Path.GetDirectoryName(loadedSplitsFile) is string dir) {
        splitsStartLocation = await StorageProvider.TryGetFolderFromPathAsync(dir);
      }
      var layoutStartLocation = defaultStartLocation;
      if (loadedLayoutFile is not null && Path.GetDirectoryName(loadedLayoutFile) is string layoutDir) {
        layoutStartLocation = await StorageProvider.TryGetFolderFromPathAsync(layoutDir);
      }

      splitsOpenOptions.SuggestedStartLocation = splitsStartLocation;
      splitsSaveOptions.SuggestedStartLocation = splitsStartLocation;

      layoutOpenOptions.SuggestedStartLocation = layoutStartLocation;
      layoutSaveOptions.SuggestedStartLocation = layoutStartLocation;
    });

    Closing += OnClosing;
    DataContext = GlobalContext;

    InitializeComponent();

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
      GlobalContext.LayoutData.Clear();
      foreach (var layoutItem in layout) {
        GlobalContext.LayoutData.Add(layoutItem);
      }
    }
    else {
      GlobalContext.LayoutData = LayoutData.Default;
      configLoader.Set(ConfigKeys.LastLoadedLayout, "");
    }

    GlobalContext.Initialize();
    GlobalContext.InitializeInputAndKeymap(configLoader);
  }
  private void OnClosing(object? sender, WindowClosingEventArgs e) {
    Input.Stop();
  }
  public void NewSplits(object sender, RoutedEventArgs e) {
    GlobalContext.Run = new();
  }
  public async void OpenSplits(object sender, RoutedEventArgs e) {
    if (GlobalContext.Timer.Running) {
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
      // TODO: needs mutex or synchronization
      GlobalContext.Run = run;
    }
    else {
      configLoader.Set(ConfigKeys.LastLoadedSplits, "");
    }
  }
  public void SaveSplits(object sender, RoutedEventArgs e) {
    if (GlobalContext.Run == null) {
      Logs.LogError("Run was null when tried to save.");
      return;
    }
    if (loadedSplitsFile == null) {
      SaveSplitsAs(sender, e);
      return;
    }
    if (serializer.Write(loadedSplitsFile, GlobalContext.Run)) {
      configLoader.Set(ConfigKeys.LastLoadedSplits, loadedSplitsFile);
    }
  }
  public async void SaveSplitsAs(object sender, RoutedEventArgs e) {
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
  public async void EditSplits(object sender, RoutedEventArgs e) {
    if (GlobalContext.Run == null) {
      NewSplits(sender, e);
    }
    var window = new RunEditor(GlobalContext.Run);
    GlobalContext.StartEditing();
    await window.ShowDialog(this);
    GlobalContext.StopEditing();
    GlobalContext.Run = window.GetRun() ?? new();
  }
  public async void EditKeybinds(object sender, RoutedEventArgs e) {
    GlobalContext.StartEditing();
    var win = new KeybindsEditor(GlobalContext);
    await win.ShowDialog(this);
    GlobalContext.StopEditing();
    configLoader.Set("keymap", GlobalContext.keymap);
  }
  public void NewLayout(object sender, RoutedEventArgs e) {
    GlobalContext.LayoutData = LayoutData.Default;
  }
  public async void OpenLayout(object sender, RoutedEventArgs e) {
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
      GlobalContext.LayoutData.Clear();
      foreach (var layoutItem in layout) {
        GlobalContext.LayoutData.Add(layoutItem);
      }
    }
    else {
      configLoader.Set(ConfigKeys.LastLoadedLayout, "");
    }
  }
  public void SaveLayout(object sender, RoutedEventArgs e) {
    if (loadedLayoutFile == null) {
      SaveLayoutAs(sender, e);
      return;
    }
    if (serializer.Write(loadedLayoutFile, GlobalContext.LayoutData)) {
      configLoader.Set(ConfigKeys.LastLoadedLayout, loadedLayoutFile);
    }
  }
  public async void SaveLayoutAs(object sender, RoutedEventArgs e) {
    if (loadedLayoutFile is not null && Path.GetDirectoryName(loadedLayoutFile) is string dir) {
      layoutSaveOptions.SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(dir);
    }
    var file = await StorageProvider.SaveFilePickerAsync(layoutSaveOptions);
    if (file == null) {
      Logs.LogInfo("You must select a file to 'Save As'");
      return;
    }
    var path = file.Path.AbsolutePath;
    if (serializer.Write(path, GlobalContext.LayoutData)) {
      loadedLayoutFile = path;
      configLoader.Set(ConfigKeys.LastLoadedLayout, loadedLayoutFile);
    }
  }
  public async void EditLayout(object sender, RoutedEventArgs e) {
    var window = new LayoutEditor();
    void onClosing(object? sender, EventArgs args) {
      window.Close();
    }
    Closing += onClosing;
    window.Closing += delegate {
      Closing -= onClosing;
    };
    await window.ShowDialog(this);
  }
  public void CloseWindow(object sender, RoutedEventArgs e) {
    Close();
  }
}
