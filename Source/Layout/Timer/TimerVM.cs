using System.ComponentModel;
using LibreSplit.Timing;

namespace LibreSplit;

public class TimerVM : ViewModelBase {
  private Timer? timer;

  public TimerLayout LayoutItem {get;}
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
        Classes = "Ahead";
      } else {
        Classes = "Inactive";
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
  private string classes = "Inactive";

  public TimerVM(TimerLayout layoutItem) {
    LayoutItem = layoutItem;
  }

  public string Classes {
    get => classes;
    set {
      if (classes != value) {
        classes = value;
        OnPropertyChanged();
      }
    }
  }
}