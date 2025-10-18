using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace LibreSplit.Timing;

/// <summary>
/// This class defines a single segment of a greater run and holds display information and timing data.
/// </summary>
/// <param name="label"></param>
public class SegmentData(string label = "New Split") : INotifyPropertyChanged {
  private TimeSpan? _segmentTime;
  [JsonIgnore]
  public TimeSpan? SegmentTime {
    get { return _segmentTime; }
    set {
      if (_segmentTime != value) {
        _segmentTime = value;
        OnPropertyChanged();
      }
    }
  }

  private string label = label;
  public string Label {
    get => label;
    set {
      label = value;
      OnPropertyChanged();
    }
  }


  private TimeSpan? _splitTime;
  [JsonIgnore]
  public TimeSpan? SplitTime {
    get { return _splitTime; }
    set {
      if (_splitTime != value) {
        _splitTime = value;
        OnPropertyChanged();
      }
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;
  protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  public List<TimeSpan> SegmentTimeHistory { get; set; } = [];
  public List<TimeSpan> SplitTimeHistory { get; set; } = [];


  private TimeSpan? pBSegmentTime = null;
  public TimeSpan? PBSegmentTime {
    get => pBSegmentTime;
    set {
      pBSegmentTime = value;
      OnPropertyChanged();
    }
  }

  private TimeSpan? pBSplitTime = null;
  public TimeSpan? PBSplitTime {
    get => pBSplitTime;
    set {
      pBSplitTime = value;
      OnPropertyChanged();
    }
  }
  public void AddSegmentTime(TimeSpan delta) {
    SegmentTimeHistory.Add(delta);
  }
  public void AddSplitTime(TimeSpan time) {
    SplitTimeHistory.Add(time);
  }

  public TimeSpan? BestSegmentTime() {
    if (SegmentTimeHistory.Count == 0) {
      return null;
    }
    return SegmentTimeHistory.Min();
  }
  public TimeSpan? WorstSegmentTime() {
    if (SegmentTimeHistory.Count == 0) {
      return null;
    }
    return SegmentTimeHistory.Max();
  }
  public TimeSpan? AverageSegmentTime() {
    if (SegmentTimeHistory.Count == 0) {
      return null;
    }
    long averageTicks = (long)SegmentTimeHistory.Average(timeSpan => timeSpan.Ticks);
    return new TimeSpan(averageTicks);
  }

  public TimeSpan? BestSplitTime() {
    if (SplitTimeHistory.Count == 0) {
      return null;
    }
    return SplitTimeHistory.Min();
  }
  public TimeSpan? WorstSplitTime() {
    if (SplitTimeHistory.Count == 0) {
      return null;
    }
    return SplitTimeHistory.Max();
  }
  public TimeSpan? AverageSplitTime() {
    if (SplitTimeHistory.Count == 0) {
      return null;
    }
    long averageTicks = (long)SplitTimeHistory.Average(timeSpan => timeSpan.Ticks);
    return new TimeSpan(averageTicks);
  }

  public static SegmentData Default => new();
}


