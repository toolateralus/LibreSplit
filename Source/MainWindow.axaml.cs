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

namespace LibreSplit;

public partial class MainWindow : Window {
  static FilePickerOpenOptions openOptions = new();
  static FilePickerSaveOptions saveOptions = new();
  private readonly Timer timer = new();
  private RunData? Run { get; set; }
  private string? loadedFile;
  private readonly ConfigLoader configLoader = new();
  private readonly Serializer serializer = new();
  MainWindowVM viewModel = new();

  public TimeSpan CurrentTime {
    get;
    set;
  } = TimeSpan.Zero;
  public MainWindow() {
    DataContext = viewModel;
    InitializeComponent();
    KeyDown += HandleInput;

    configLoader.LoadOrCreate();
    configLoader.TryLoadSplits(out loadedFile);

    if (loadedFile != null) {
      Run = serializer.ReadRunData(loadedFile, configLoader);
      viewModel.Layout.Add(new SplitsControl(viewModel));
      viewModel.Layout.Add(new TimerControl(viewModel));
      viewModel.Segments = Run.Segments;
    }
    
    timer.AttachUpdateHook(elapsedSpan => {
      CurrentTime = elapsedSpan;
      if (Run?.SegmentIndex < Run?.Segments.Count) {
        Run.Segments[Run.SegmentIndex].SegmentTime = timer.Delta;
        Run.Segments[Run.SegmentIndex].SplitTime = timer.Elapsed;
        viewModel.Elapsed = timer.Elapsed;
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
              viewModel.ClearActiveSegment();
            } else {
              viewModel.SetActiveSegment(Run.Segments[Run.SegmentIndex]);
            }
          }
          else {
            Run.Start(timer);
            viewModel.SetActiveSegment(Run.Segments[Run.SegmentIndex]);
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
          viewModel.ClearActiveSegment();
          viewModel.Elapsed = TimeSpan.Zero;

        }
        break;
    }
  }

  public async void NewFileMethod(object sender, RoutedEventArgs e) {
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
    loadedFile = path;
    await serializer.WriteRunData(path, Run, configLoader);
  }
  public async void OpenFileMethod(object sender, RoutedEventArgs e) {
    var list = await StorageProvider.OpenFilePickerAsync(openOptions);
    if (list.Count == 0) {
      return;
    }
    loadedFile = list[0].Path?.AbsolutePath!;
    
    if (loadedFile != null)
      serializer.ReadRunData(loadedFile, configLoader);
  }
  public async void SaveFileMethod(object sender, RoutedEventArgs e) {
    if (loadedFile == null) {
      Console.WriteLine("You must select a file to 'Save'");
      return;
    }
    if (Run == null) {
      Console.WriteLine("Run was null when tried to save.");
      return;
    }
    await serializer.WriteRunData(loadedFile, Run, configLoader);
  
  }
  public void CloseWindow(object sender, RoutedEventArgs e) {
    Close();
  }
  public async void SaveAsFileMethod(object sender, RoutedEventArgs e) {
    var file = await StorageProvider.SaveFilePickerAsync(saveOptions);
    if (file == null) {
      Console.WriteLine("You must select a file to 'Save As'");
      return;
    }
    var path = file.Path.AbsolutePath;
    
    if (Run == null) {
      Console.WriteLine("Run was null when tried to save.");
      return;
    }
    
    await serializer.WriteRunData(path, Run, configLoader);
  }
  public async void EditSplitsMethod(object sender, RoutedEventArgs e) {
    if (Run == null) {
      NewFileMethod(sender, e);
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
  public void ThemesMethod(object sender, RoutedEventArgs e) {

  }
  #endregion
}