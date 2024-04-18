using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using LibreSplit.Controls;
using LibreSplit.Timing;
using Newtonsoft.Json;

namespace LibreSplit;

public partial class MainWindow : Window {
    static FilePickerOpenOptions openOptions = new() {
    };
    static FilePickerSaveOptions saveOptions = new() {
    };
    private readonly Timer timer = new();
    private RunData? Run { get; set; }
    private IStorageFile? loadedFile;
    public MainWindow() {
        InitializeComponent();
        KeyDown += HandleInput;
    }
  
  private void HandleInput(object? sender, KeyEventArgs e) {
    switch (e.Key) {
      case Key.D1: {
        
      } break;
      case Key.D2: {
        
      } break;
      case Key.D3: {
        
      } break;
      case Key.D4: {
        
      } break;
      case Key.D5: {
        
      } break;
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
        loadedFile = newFilePath;
        await WriteRunData(path);
    }

    public async void OpenFileMethod(object sender, RoutedEventArgs e) {
        var list = await StorageProvider.OpenFilePickerAsync(openOptions);
        if (list.Count == 0) {
            return;
        }
        loadedFile = list[0];
        ReadRunData(loadedFile.Path.AbsolutePath);
    }
    
    public async void SaveFileMethod(object sender, RoutedEventArgs e) {
        if (loadedFile == null) {
            Console.WriteLine("You must select a file to 'Save'");
            return;
        }
        var path = loadedFile.Path.AbsolutePath;
        await WriteRunData(path);

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

    private async Task WriteRunData(string path) {
        path = Uri.UnescapeDataString(path);
        using var stream = new StreamWriter(path);
        try {
            await stream.WriteAsync(JsonConvert.SerializeObject(Run, Formatting.Indented));
        }
        catch (JsonSerializationException e ){
            Console.WriteLine($"An error has occured while serializing segment data. {e}");
        }
        stream.Close();
    }
    
    public void ReadRunData(string path) {
        path = Uri.UnescapeDataString(path);
        if (!File.Exists(path)) {
            throw new FileNotFoundException($"Couldn't find file {path}");
        }
        var text = File.ReadAllText(path);
        Run = JsonConvert.DeserializeObject<RunData>(text) ?? throw new Exception("Failed to deserialize run data.");
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
    }

    public void ThemesMethod(object sender, RoutedEventArgs e) {
        throw new NotImplementedException();
    }
}