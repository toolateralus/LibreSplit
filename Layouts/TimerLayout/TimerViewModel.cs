using System;
using System.ComponentModel;
using LibreSplit.Timing;

namespace LibreSplit.Layouts.TimerLayout;

public class TimerViewModel : ViewModelBase {
  private Timer? timer;

  public TimerLayoutData LayoutItem { get; }
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
        if (!Timer.Running) {
          Classes = "Inactive";
        }
        break;
      case nameof(Timer.Elapsed):
        if (MainWindow.GlobalContext.ActiveSegment is SegmentData activeSegment) {
          if (activeSegment.PBSplitTime is null) {
            TimeSpan? nextSplit = null;
            RunData run = MainWindow.GlobalContext.Run;
            for (var i = run.SegmentIndex + 1; i < run.Segments.Count; i++) {
              if (run.Segments[i].PBSplitTime is TimeSpan split) {
                nextSplit = split;
                break;
              }
            }
            if (nextSplit is null || Timer.Elapsed < nextSplit) {
              Classes = "AheadGainingTime";
            }
            else {
              Classes = "BehindGainingTime";
            }
          }
          else if (Timer.Elapsed < activeSegment.PBSplitTime) {
            if (activeSegment.PBSegmentTime is null || Timer.Delta < activeSegment.PBSegmentTime) {
              Classes = "AheadGainingTime";
            }
            else {
              Classes = "AheadLosingTime";
            }
          }
          else {
            if (activeSegment.PBSegmentTime is null || Timer.Delta < activeSegment.PBSegmentTime) {
              Classes = "BehindGainingTime";
            }
            else {
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

  public TimerViewModel(TimerLayoutData layoutItem) {
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
