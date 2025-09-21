using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using SharpHook.Data;

namespace LibreSplit;

public partial class KeybindsEditor : Window, INotifyPropertyChanged {

  public new event PropertyChangedEventHandler? PropertyChanged;
  protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  private string _startOrSplit = "1";
  public string StartOrSplit {
    get => _startOrSplit;
    set {
      if (_startOrSplit != value) {
        _startOrSplit = value;
        OnPropertyChanged();
      }
    }
  }

  private string _pause = "2";
  public string Pause {
    get => _pause;
    set {
      if (_pause != value) {
        _pause = value;
        OnPropertyChanged();
      }
    }
  }

  private string _skipBack = "3";
  public string SkipBack {
    get => _skipBack;
    set {
      if (_skipBack != value) {
        _skipBack = value;
        OnPropertyChanged();
      }
    }
  }

  private string _skipForward = "4";
  public string SkipForward {
    get => _skipForward;
    set {
      if (_skipForward != value) {
        _skipForward = value;
        OnPropertyChanged();
      }
    }
  }

  private string _reset = "5";
  public string Reset {
    get => _reset;
    set {
      if (_reset != value) {
        _reset = value;
        OnPropertyChanged();
      }
    }
  }

  private CancellationTokenSource? _cancellationSource;
  private Task<string> AwaitKeyPress(CancellationToken ctk, Button button) {
    var tcs = new TaskCompletionSource<string>();

    void set(KeyCode code) {
      Input.AnyKeyPressed -= set;
      tcs.TrySetResult(code.ToString()[2..]);
    }

    int secondsRemaining = 3;

    button.Content = $"{secondsRemaining}...";

    var timer = new System.Timers.Timer(1000);

    timer.Elapsed += (s, e) => {
      secondsRemaining--;
      Dispatcher.UIThread.Invoke(() => {
        if (secondsRemaining > 0)
          button.Content = $"{secondsRemaining}...";
      });

      if (secondsRemaining <= 0) {
        timer.Stop();
      }
    };

    timer.Start();

    Input.AnyKeyPressed += set;

    ctk.Register(() => {
      Dispatcher.UIThread.Invoke(() => {
        button.Content = "Listen";
      });
      timer.Stop();
      timer.Dispose();
      Input.AnyKeyPressed -= set;
      tcs.TrySetCanceled();
    });

    return tcs.Task;
  }

  public void SetControlsEnabled(bool state) {
    StartOrSplitButton.IsEnabled = state;
    PauseButton.IsEnabled = state;
    SkipBackButton.IsEnabled = state;
    SkipForwardButton.IsEnabled = state;
    ResetButton.IsEnabled = state;
  }


  public async void ListenForBinding(object? sender, Avalonia.Interactivity.RoutedEventArgs a) {
    if (sender is not Button button) {
      return;
    }

    SetControlsEnabled(false);
    _cancellationSource?.Cancel();
    _cancellationSource = new CancellationTokenSource(3000);
    try {
      var key = await AwaitKeyPress(_cancellationSource.Token, button);
      switch (button.Name) {
        case "StartOrSplitButton": StartOrSplit = key; break;
        case "PauseButton": Pause = key; break;
        case "SkipBackButton": SkipBack = key; break;
        case "SkipForwardButton": SkipForward = key; break;
        case "ResetButton": Reset = key; break;
        default: break;
      }
    }
    catch (TaskCanceledException) { }
    finally {
      SetControlsEnabled(true);
      button.Content = "Listen";
      _cancellationSource.Cancel();
      _cancellationSource.Dispose();
      _cancellationSource = null;
    }
  }

  public KeybindsEditor(LibreSplitContext context) {
    InitializeComponent();
    DataContext = this;

    static string keyCodeDisplayString(KeyCode kc) => kc.ToString()[2..];

    StartOrSplit = keyCodeDisplayString(context.keymap[Keybind.StartOrSplit]);
    Pause = keyCodeDisplayString(context.keymap[Keybind.Pause]);
    SkipBack = keyCodeDisplayString(context.keymap[Keybind.SkipBack]);
    SkipForward = keyCodeDisplayString(context.keymap[Keybind.SkipForward]);
    Reset = keyCodeDisplayString(context.keymap[Keybind.Reset]);

    Closing += delegate {
      context.keymap = new()
      {
        {Keybind.StartOrSplit, Input.StringToKeyCode(StartOrSplit)},
        {Keybind.Pause, Input.StringToKeyCode(Pause)},
        {Keybind.SkipBack, Input.StringToKeyCode(SkipBack)},
        {Keybind.SkipForward, Input.StringToKeyCode(SkipForward)},
        {Keybind.Reset, Input.StringToKeyCode(Reset)},
      };

      Console.WriteLine("Key binding map updated");
      foreach (var binding in context.keymap) {
        Console.WriteLine($"{binding.Key} {binding.Value}");
      }
    };
  }
}
