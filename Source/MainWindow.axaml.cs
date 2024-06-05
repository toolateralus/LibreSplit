using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using LibreSplit.IO.Config;
using LibreSplit.Controls;
using LibreSplit.Timing;
using LibreSplit.IO.Serialization;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace LibreSplit;

public partial class MainWindow : Window {
  static FilePickerOpenOptions openOptions = new();
  static FilePickerSaveOptions saveOptions = new();
  private string? loadedSplitsFile;
  private string? loadedLayoutFile;
  private readonly ConfigLoader configLoader = new();
  private readonly Serializer serializer = new();
  public static LibreSplitContext GlobalContext {get; set;} = new();
  public MainWindow() {
    Closing += OnClosing;
   
    DataContext = GlobalContext;
    InitializeComponent();
    configLoader.LoadOrCreate();
    configLoader.TryLoadSplits(out loadedSplitsFile);
    if (loadedSplitsFile != null&& serializer.Read<RunData>(loadedSplitsFile, out var run)) {
      GlobalContext.Run = run;
    } else {
      configLoader.Set(ConfigKeys.LastLoadedSplits, "");
    }
    configLoader.TryLoadLayout(out loadedLayoutFile);
    if (loadedLayoutFile != null && serializer.Read<Layout>(loadedLayoutFile, out var layout)) {
      GlobalContext.Layout.Clear();
      foreach(var layoutItem in layout) {
        GlobalContext.Layout.Add(layoutItem);
      }
    } else {
      configLoader.Set(ConfigKeys.LastLoadedLayout, "");
    }
    
    GlobalContext.Initialize();
    
    GlobalContext.InitializeInputAndKeymap(configLoader);
    
    Input.KeyDown += GlobalContext.HandleInput;
  }
  private void OnClosing(object? sender, WindowClosingEventArgs e) {
    Input.Stop();
  }
  #region Events
  public async void NewSplits(object sender, RoutedEventArgs e) {
    GlobalContext.Run = new();
    var saveOptions = new FilePickerSaveOptions {
      Title = "newSplits.json",
      FileTypeChoices = [
            new(".json"),
            ]
    };

    var newFilePath = await StorageProvider.SaveFilePickerAsync(saveOptions);
    if (newFilePath == null) {
      return;
    }

    var path = newFilePath.Path.AbsolutePath;
    loadedSplitsFile = path;
    if (serializer.Write(path, GlobalContext.Run)) {
      configLoader.Set(ConfigKeys.LastLoadedSplits, path);
    }
  }
  public async void OpenSplits(object sender, RoutedEventArgs e) {
    var list = await StorageProvider.OpenFilePickerAsync(openOptions);
    if (list.Count == 0) {
      return;
    }
    loadedSplitsFile = list[0].Path?.AbsolutePath!;
    
    if (loadedSplitsFile != null&& serializer.Read<RunData>(loadedSplitsFile, out var run)) {
      configLoader.Set(ConfigKeys.LastLoadedSplits, loadedSplitsFile);
      GlobalContext.Run = run;
    } else {
      configLoader.Set(ConfigKeys.LastLoadedSplits, "");
    }
  }
  public void SaveSplits(object sender, RoutedEventArgs e) {
    if (loadedSplitsFile == null) {
      Console.WriteLine("You must select a file to 'Save'");
      return;
    }
    if (GlobalContext.Run == null) {
      Console.WriteLine("Run was null when tried to save.");
      return;
    }
    if (serializer.Write(loadedSplitsFile, GlobalContext.Run)) {
      configLoader.Set(ConfigKeys.LastLoadedSplits, loadedSplitsFile);
    }
  
  }
  public async void SaveSplitsAs(object sender, RoutedEventArgs e) {
    var file = await StorageProvider.SaveFilePickerAsync(saveOptions);
    if (file == null) {
      Console.WriteLine("You must select a file to 'Save As'");
      return;
    }
    if (GlobalContext.Run == null) {
      Console.WriteLine("Run was null when tried to save.");
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
    var window = new SplitEditor(GlobalContext.Run);
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
  public async void NewLayout(object sender, RoutedEventArgs e) {
    GlobalContext.Layout.Clear();
    GlobalContext.Layout.Add(new SplitsLayout());
    GlobalContext.Layout.Add(new TimerLayout());
    var saveOptions = new FilePickerSaveOptions {
      Title = "newLayout.json",
      FileTypeChoices = [
            new(".json"),
            ]
    };

    var newFilePath = await StorageProvider.SaveFilePickerAsync(saveOptions);
    if (newFilePath == null) {
      return;
    }

    var path = newFilePath.Path.AbsolutePath;
    loadedLayoutFile = path;
    if (serializer.Write(path, GlobalContext.Layout)) {
      configLoader.Set(ConfigKeys.LastLoadedLayout, path);
    }
  }
  public async void OpenLayout(object sender, RoutedEventArgs e) {
    var list = await StorageProvider.OpenFilePickerAsync(openOptions);
    if (list.Count == 0) {
      return;
    }
    loadedLayoutFile = list[0].Path?.AbsolutePath!;
    
    if (loadedLayoutFile != null && serializer.Read<Layout>(loadedLayoutFile, out var layout)) {
      configLoader.Set(ConfigKeys.LastLoadedLayout, loadedLayoutFile);
      GlobalContext.Layout.Clear();
      foreach(var layoutItem in layout) {
        GlobalContext.Layout.Add(layoutItem);
      }
    } else {
      configLoader.Set(ConfigKeys.LastLoadedLayout, "");
    }
  }
  public void SaveLayout(object sender, RoutedEventArgs e) {
    if (loadedLayoutFile == null) {
      Console.WriteLine("You must select a file to 'Save'");
      return;
    }
    if (serializer.Write(loadedLayoutFile, GlobalContext.Layout)) {
      configLoader.Set(ConfigKeys.LastLoadedLayout, loadedLayoutFile);
    }
  }
  public async void SaveLayoutAs(object sender, RoutedEventArgs e) {
    var file = await StorageProvider.SaveFilePickerAsync(saveOptions);
    if (file == null) {
      Console.WriteLine("You must select a file to 'Save As'");
      return;
    }
    var path = file.Path.AbsolutePath;
    if (serializer.Write(path, GlobalContext.Layout)) {
      loadedLayoutFile = path;
      configLoader.Set(ConfigKeys.LastLoadedLayout, loadedLayoutFile);
    }
  }
  public async void EditLayout(object sender, RoutedEventArgs e) {
    var window = new LayoutEditor();
    void onClosing(object? sender, EventArgs args) {
      window.Close();
    };
    Closing += onClosing;
    window.Closing += delegate {
      Closing -= onClosing;
    };
    await window.ShowDialog(this);
  }
  public void CloseWindow(object sender, RoutedEventArgs e) {
    Close();
  }
  public void ThemesMethod(object sender, RoutedEventArgs e) {

  }
  #endregion
}