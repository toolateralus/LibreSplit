using System.ComponentModel;
using LibreSplit.Timing;

namespace LibreSplit;

public class TimerViewModel : ViewModelBase {
  private Timer? timer;

  public TimerLayout LayoutItem { get; }
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
    case nameof(Timer.Elapsed):
      if (MainWindow.GlobalContext.ActiveSegment is SegmentData activeSegment) {
        if (Timer.Elapsed < activeSegment.PBSplitTime) {
          if (Timer.Delta < activeSegment.PBSegmentTime) {
            Classes = "AheadGainingTime";
          } else {
            Classes = "AheadLosingTime";
          }
        } else {
          if (Timer.Delta < activeSegment.PBSegmentTime) {
            Classes = "BehindGainingTime";
          } else {
            Classes = "BehindLosingTime";
          }
        }
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

  public TimerViewModel(TimerLayout layoutItem) {
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