using System;
using System.ComponentModel;
using LibreSplit.Timing;

namespace LibreSplit;

public class TimerVM : ViewModelBase {
  private TimerLayout layoutItem = new();
  private Timer? timer;
  private TimerState state = TimerState.Inactive;

  public TimerLayout LayoutItem {
    get => layoutItem;
    set {
      layoutItem = value;
      OnPropertyChanged();
    }
  }
  public Timer? Timer {
    get => timer;
    set {
      timer = value;
      OnPropertyChanged();
    }
  }
  public void OnAttachedToLogicalTree() {
    Timer = MainWindow.GlobalContext.Timer;
    Timer.PropertyChanged += TimerPropertyChanged;
  }

  private void TimerPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    if (Timer is null) {
      return;
    }
    switch (e.PropertyName) {
    case nameof(Timer.Running):
      if (Timer.Running) {
        State = TimerState.Ahead;
      } else {
        State = TimerState.Inactive;
      }
      break;
    }
  }

  public void OnDetachedFromLogicalTree() {
    if (Timer != null) {
      Timer.PropertyChanged -= TimerPropertyChanged;
      Timer = null;
    }
  }
  public TimerState State {
    get => state; set {
      if (state != value) {
      state = value;
      OnPropertyChanged();
      }
    }
  }
}