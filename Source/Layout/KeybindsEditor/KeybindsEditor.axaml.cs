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

  public void MakeAllControlsDisabled() {
    StartOrSplitBox.IsEnabled = false;
    StartOrSplitButton.IsEnabled = false;

    PauseBox.IsEnabled = false;
    PauseButton.IsEnabled = false;

    SkipBackBox.IsEnabled = false;
    SkipBackButton.IsEnabled = false;

    SkipForwardBox.IsEnabled = false;
    SkipForwardButton.IsEnabled = false;

    ResetBox.IsEnabled = false;
    ResetButton.IsEnabled = false;
  }

  public void MakeAllControlsEnabled() {
    StartOrSplitBox.IsEnabled = true;
    StartOrSplitButton.IsEnabled = true;

    PauseBox.IsEnabled = true;
    PauseButton.IsEnabled = true;

    SkipBackBox.IsEnabled = true;
    SkipBackButton.IsEnabled = true;

    SkipForwardBox.IsEnabled = true;
    SkipForwardButton.IsEnabled = true;

    ResetBox.IsEnabled = true;
    ResetButton.IsEnabled = true;
  }


  public async void ListenForBinding_StartOrSplit(object? sender, Avalonia.Interactivity.RoutedEventArgs a) {
    MakeAllControlsDisabled();
    _cancellationSource?.Cancel();
    _cancellationSource = new CancellationTokenSource(3000);
    try {
      StartOrSplit = await AwaitKeyPress(_cancellationSource.Token, StartOrSplitButton);
    }
    catch (TaskCanceledException) { }
    finally {
      MakeAllControlsEnabled();
      StartOrSplitButton.Content = "Listen";
      _cancellationSource.Cancel();
      _cancellationSource.Dispose();
      _cancellationSource = null;
    }
  }

  public async void ListenForBinding_Pause(object? sender, Avalonia.Interactivity.RoutedEventArgs a) {
    MakeAllControlsDisabled();
    _cancellationSource?.Cancel();
    _cancellationSource = new CancellationTokenSource(3000);
    try {
      Pause = await AwaitKeyPress(_cancellationSource.Token, PauseButton);
    }
    catch (TaskCanceledException) { }
    finally {
      MakeAllControlsEnabled();
      PauseButton.Content = "Listen";
      _cancellationSource.Cancel();
      _cancellationSource.Dispose();
      _cancellationSource = null;
    }
  }

  public async void ListenForBinding_SkipForward(object? sender, Avalonia.Interactivity.RoutedEventArgs a) {
    MakeAllControlsDisabled();
    _cancellationSource?.Cancel();
    _cancellationSource = new CancellationTokenSource(3000);
    try {
      SkipForward = await AwaitKeyPress(_cancellationSource.Token, SkipForwardButton);
    }
    catch (TaskCanceledException) { }
    finally {
      MakeAllControlsEnabled();
      SkipForwardButton.Content = "Listen";
      _cancellationSource.Cancel();
      _cancellationSource.Dispose();
      _cancellationSource = null;
    }
  }

  public async void ListenForBinding_SkipBack(object? sender, Avalonia.Interactivity.RoutedEventArgs a) {
    MakeAllControlsDisabled();
    _cancellationSource?.Cancel();
    _cancellationSource = new CancellationTokenSource(3000);
    try {
      SkipBack = await AwaitKeyPress(_cancellationSource.Token, SkipBackButton);
    }
    catch (TaskCanceledException) { }
    finally {
      MakeAllControlsEnabled();
      SkipBackButton.Content = "Listen";
      _cancellationSource.Cancel();
      _cancellationSource.Dispose();
      _cancellationSource = null;
    }
  }

  public async void ListenForBinding_Reset(object? sender, Avalonia.Interactivity.RoutedEventArgs a) {
    MakeAllControlsDisabled();
    _cancellationSource?.Cancel();
    _cancellationSource = new CancellationTokenSource(3000);
    try {
      Reset = await AwaitKeyPress(_cancellationSource.Token, ResetButton);
    }
    catch (TaskCanceledException) { }
    finally {
      MakeAllControlsEnabled();
      ResetButton.Content = "Listen";
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
