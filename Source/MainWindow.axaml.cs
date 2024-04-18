using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Themes.Simple;
using Avalonia.Threading;
using LibreSplit.Config;
using LibreSplit.Controls;
using LibreSplit.Timing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LibreSplit;

public partial class MainWindow : Window {
  static FilePickerOpenOptions openOptions = new() {
  };
  static FilePickerSaveOptions saveOptions = new() {
  };
  private readonly Timer timer = new();
  private RunData? Run { get; set; }
  private string? loadedFile;
  private readonly ConfigLoader configLoader = new();

  public TimeSpan CurrentTime {
    get;
    set;
  } = TimeSpan.Zero;


  public MainWindow() {
    InitializeComponent();
    KeyDown += HandleInput;

    configLoader.LoadOrCreate();
    configLoader.TryLoadSplits(out loadedFile);

    if (loadedFile != null) {
      ReadRunData(loadedFile);
    }
    
    timer.AttachUpdateHook(elapsedSpan => {
      CurrentTime = elapsedSpan;
      if (Run?.SegmentIndex < Run?.Segments.Count) {
        Run.Segments[Run.SegmentIndex].CurrentDelta = timer.Delta;
        Run.Segments[Run.SegmentIndex].CurrentSplitTime = timer.Elapsed;
      }
    });
  }
  private void ReadRunData(string path) {
    configLoader.Set(ConfigKeys.LastLoadedSplits, path);
    
    path = Uri.UnescapeDataString(path);
    
    if (!File.Exists(path)) {
      throw new FileNotFoundException($"Couldn't find file {path}");
    }
    var text = File.ReadAllText(path);
    Run = JsonConvert.DeserializeObject<RunData>(text) ?? throw new Exception("Failed to deserialize run data.");
    splitListBox.ItemsSource = Run.Segments;
  }
  private async Task WriteRunData(string path) {
    configLoader.Set(ConfigKeys.LastLoadedSplits, path);
    
    path = Uri.UnescapeDataString(path);
    
    using var stream = new StreamWriter(path);
    
    try {
      await stream.WriteAsync(JsonConvert.SerializeObject(Run, Formatting.Indented));
    }
    catch (JsonSerializationException e) {
      Console.WriteLine($"An error has occured while serializing segment data. {e}");
    }
    stream.Close();
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
            }
          }
          else {
            timer.Start();
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
    await WriteRunData(path);
  }
  public async void OpenFileMethod(object sender, RoutedEventArgs e) {
    var list = await StorageProvider.OpenFilePickerAsync(openOptions);
    if (list.Count == 0) {
      return;
    }
    loadedFile = list[0].Path?.AbsolutePath!;

    if (loadedFile != null)
      ReadRunData(loadedFile);
  }
  public async void SaveFileMethod(object sender, RoutedEventArgs e) {
    if (loadedFile == null) {
      Console.WriteLine("You must select a file to 'Save'");
      return;
    }
    await WriteRunData(loadedFile);

  }
  public async void SaveAsFileMethod(object sender, RoutedEventArgs e) {
    var file = await StorageProvider.SaveFilePickerAsync(saveOptions);
    if (file == null) {
      Console.WriteLine("You must select a file to 'Save As'");
      return;
    }
    var path = file.Path.AbsolutePath;
    await WriteRunData(path);
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