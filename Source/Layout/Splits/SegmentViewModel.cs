using System;
using System.ComponentModel;
using LibreSplit.Timing;

namespace LibreSplit;

public enum SegmentStatus {
  Passed,
  Current,
  Upcoming,
}

public class SegmentViewModel : ViewModelBase {
  public SegmentData Segment { get; }
  private bool isActive = false;
  public bool IsActive {
    get => isActive;
    set {
      isActive = value;
      OnPropertyChanged();
    }
  }
  private TimeSpan? splitTime;
  public TimeSpan? SplitTime {
    get => splitTime;
    set {
      splitTime = value;
      OnPropertyChanged();
    }
  }
  private TimeSpan? segmentTime;
  public TimeSpan? SegmentTime {
    get => segmentTime;
    set {
      segmentTime = value;
      OnPropertyChanged();
    }
  }
  public void SetStatus(SegmentStatus status) {
    switch (status) {
      case SegmentStatus.Passed:
        IsActive = false;
        SplitTime = Segment.SplitTime;
        SegmentTime = Segment.SegmentTime;
        break;
      case SegmentStatus.Current:
        IsActive = true;
        SplitTime = Segment.PBSplitTime;
        SegmentTime = Segment.PBSegmentTime;
        break;
      case SegmentStatus.Upcoming:
        IsActive = false;
        SplitTime = Segment.PBSplitTime;
        SegmentTime = Segment.PBSegmentTime;
        break;
    }
  }

  public SegmentViewModel(SegmentData segmentData) {
    Segment = segmentData;
    SetStatus(SegmentStatus.Upcoming);
  }
}