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

namespace LibreSplit;

public partial class MainWindow : Window {
  static FilePickerOpenOptions openOptions = new();
  static FilePickerSaveOptions saveOptions = new();
  private readonly Timer timer = new();
  private RunData? Run { get; set; }
  private string? loadedSplitsFile;
  private string? loadedLayoutFile;
  private readonly ConfigLoader configLoader = new();
  private readonly Serializer serializer = new();
  public static LibreSplitContext GlobalContext = new();

  public TimeSpan CurrentTime {
    get;
    set;
  } = TimeSpan.Zero;
  public MainWindow() {
    DataContext = GlobalContext;
    InitializeComponent();
    KeyDown += HandleInput;
    GlobalContext.Layout.Add(new SplitsLayout());
    GlobalContext.Layout.Add(new TimerLayout());

    configLoader.LoadOrCreate();
    configLoader.TryLoadSplits(out loadedSplitsFile);
    if (loadedSplitsFile != null&& serializer.Read<RunData>(loadedSplitsFile, out var run)) {
      Run = run;
      GlobalContext.Run = Run;
    } else {
      configLoader.Set(ConfigKeys.LastLoadedSplits, "");
    }
    configLoader.TryLoadLayout(out loadedLayoutFile);
    if (loadedLayoutFile != null && serializer.Read<List<LayoutItem>>(loadedLayoutFile, out var layout)) {
      GlobalContext.Layout.Clear();
      foreach(var layoutItem in layout) {
        GlobalContext.Layout.Add(layoutItem);
      }
    } else {
      configLoader.Set(ConfigKeys.LastLoadedLayout, "");
    }
    
    timer.AttachUpdateHook(elapsedSpan => {
      CurrentTime = elapsedSpan;
      if (Run?.SegmentIndex < Run?.Segments.Count) {
        Run.Segments[Run.SegmentIndex].SegmentTime = timer.Delta;
        Run.Segments[Run.SegmentIndex].SplitTime = timer.Elapsed;
        GlobalContext.Elapsed = timer.Elapsed;
      }
    });
  }
  

  #region Events
  private void HandleInput(object? sender, KeyEventArgs e) {
    if (Run == null) {
      return;
    }
    switch (e.Key) {
      case Key.D1: {
          if (timer.Running) {
            // this returns false at the end of the run.
            if (!Run.Split(timer)) {
              timer.Stop();
              GlobalContext.ClearActiveSegment();
            } else {
              GlobalContext.SetActiveSegment(Run.Segments[Run.SegmentIndex]);
            }
          }
          else {
            Run.Start(timer);
            GlobalContext.SetActiveSegment(Run.Segments[Run.SegmentIndex]);
          }

        }
        break;
      case Key.D2: {
          // todo: Implement pausing.
        }
        break;
      case Key.D3: {
          // todo: implement skipping back
        }
        break;
      case Key.D4: {
          // todo: implement skipping forward.
        }
        break;
      case Key.D5: {
          timer.Reset();
          Run.Reset();
          GlobalContext.ClearActiveSegment();
          GlobalContext.Elapsed = TimeSpan.Zero;

        }
        break;
    }
  }

  public async void NewSplits(object sender, RoutedEventArgs e) {
    Run = new();
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
    if (serializer.Write(path, Run)) {
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
      Run = run;
      GlobalContext.Run = Run;
    } else {
      configLoader.Set(ConfigKeys.LastLoadedSplits, "");
    }
  }
  public void SaveSplits(object sender, RoutedEventArgs e) {
    if (loadedSplitsFile == null) {
      Console.WriteLine("You must select a file to 'Save'");
      return;
    }
    if (Run == null) {
      Console.WriteLine("Run was null when tried to save.");
      return;
    }
    if (serializer.Write(loadedSplitsFile, Run)) {
      configLoader.Set(ConfigKeys.LastLoadedSplits, loadedSplitsFile);
    }
  
  }
  public async void SaveSplitsAs(object sender, RoutedEventArgs e) {
    var file = await StorageProvider.SaveFilePickerAsync(saveOptions);
    if (file == null) {
      Console.WriteLine("You must select a file to 'Save As'");
      return;
    }
    if (Run == null) {
      Console.WriteLine("Run was null when tried to save.");
      return;
    }
    if (serializer.Write(file.Path.AbsolutePath, Run)) {
      loadedSplitsFile = file.Path.AbsolutePath;
      configLoader.Set(ConfigKeys.LastLoadedSplits, loadedSplitsFile);
    }
  }
  public async void EditSplits(object sender, RoutedEventArgs e) {
    if (Run == null) {
      NewSplits(sender, e);
    }

    var window = new SplitEditor(Run);
    void onClosing(object? sender, EventArgs args) {
      window.Close();
    };
    Closing += onClosing;
    window.Closing += delegate {
      Closing -= onClosing;
    };
    await window.ShowDialog(this);

    Run = window.GetRun();
  }
  public async void NewLayout(object sender, RoutedEventArgs e) {
    GlobalContext.Layout.Clear();
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
    if (serializer.Write(path, GlobalContext.Layout.ToList())) {
      configLoader.Set(ConfigKeys.LastLoadedLayout, path);
    }
  }
  public async void OpenLayout(object sender, RoutedEventArgs e) {
    var list = await StorageProvider.OpenFilePickerAsync(openOptions);
    if (list.Count == 0) {
      return;
    }
    loadedLayoutFile = list[0].Path?.AbsolutePath!;
    
    if (loadedLayoutFile != null && serializer.Read<List<LayoutItem>>(loadedLayoutFile, out var layout)) {
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
    if (serializer.Write(loadedLayoutFile, GlobalContext.Layout.ToList())) {
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
    if (serializer.Write(path, GlobalContext.Layout.ToList())) {
      loadedLayoutFile = path;
      configLoader.Set(ConfigKeys.LastLoadedLayout, loadedLayoutFile);
    }
  }
  public async void EditLayout(object sender, RoutedEventArgs e) {

  }
  public void CloseWindow(object sender, RoutedEventArgs e) {
    Close();
  }
  public void ThemesMethod(object sender, RoutedEventArgs e) {

  }
  #endregion
}