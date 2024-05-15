using System;
using Avalonia.Input;
using LibreSplit.Timing;

namespace LibreSplit;
public class LibreSplitContext : ViewModelBase {
  private RunData run = new();
  private SegmentData? activeSegment;
  private Layout layout = Layout.Default;

  public Timer Timer { get; } = new();
  public Layout Layout {
    get => layout;
    set {
      layout = value;
      OnPropertyChanged();
    }
  }
  public RunData Run {
    get => run;
    set {
      run = value;
      OnPropertyChanged();
    }
  }
  public SegmentData? ActiveSegment {
    get => activeSegment;
    set {
      activeSegment = value;
      OnPropertyChanged();
    }
  }

  public void SetActiveSegment(SegmentData segment) {
    ActiveSegment = segment;
  }
  public void ClearActiveSegment() {
    ActiveSegment = null;
  }

  internal void Initialize() {
    Timer.AttachUpdateHook(elapsedSpan => {
      if (Run?.SegmentIndex < Run?.Segments.Count) {
        Run.Segments[Run.SegmentIndex].SegmentTime = Timer.Delta;
        Run.Segments[Run.SegmentIndex].SplitTime = Timer.Elapsed;
      }
    });
  }

  internal void HandleInput(string key) {
    if (Run == null) {
      return;
    }
    switch (key) {
      case "1": {
          if (Timer.Running) {
            // this returns false at the end of the run.
            if (!Run.Split(Timer)) {
              Timer.Stop();
              ClearActiveSegment();
            }
            else {
              SetActiveSegment(Run.Segments[Run.SegmentIndex]);
            }
          }
          else {
            Run.Start(Timer);
            SetActiveSegment(Run.Segments[Run.SegmentIndex]);
          }

        }
        break;
      case "2": {
          // todo: Implement pausing.
        }
        break;
      case "3": {
          // todo: implement skipping back
        }
        break;
      case "4": {
          // todo: implement skipping forward.
        }
        break;
      case "5": {
          Timer.Reset();
          Run.Reset();
          ClearActiveSegment();
        }
        break;
    }
  }
}