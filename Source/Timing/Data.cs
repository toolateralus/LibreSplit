using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LibreSplit.Timing;
/// <summary>
/// This class represents a group of segments which make up a splittable timeable 'Run'.
/// </summary>
public class RunData(TimeSpan? startTime = null) {
  /// <summary>
  /// The collection of segment data that defines this run.
  /// </summary>
  public ObservableCollection<SegmentData> Segments { get; } = [];
  /// <summary>
  /// The best time achieved of an RTA attempt at these segments.
  /// </summary>
  [JsonIgnore]
  public TimeSpan? PersonalBest => Segments.LastOrDefault()?.PBSplitTime;

  
  // for binding.
  public TimeSpan StartTime {
    get;
    set;
  } = startTime ?? TimeSpan.Zero;

  /// <summary>
  /// The total count of attempts made at this run.
  /// </summary>
  public uint AttemptCount { get; set; } = 0;

  /// <summary>
  /// The current segment index of an in-progress run.
  /// </summary>
  [JsonIgnore]
  public int SegmentIndex { get; private set; } = 0;

  public void Start(Timer timer) {
    SegmentIndex = 0;
    timer.Start(startTime);
  }

  /// <summary>
  /// Advance to the next segment in the run and save current time data,
  /// </summary>
  /// <param name="timer"></param>
  /// <returns>True if the run still has remaining segments, false if the run is complete.</returns>
  public bool Split(Timer timer) {
    SegmentData segmentData = Segments[SegmentIndex];
    TimeSpan elapsed = timer.Elapsed;
    TimeSpan delta = timer.Delta;

    segmentData.SplitTime = elapsed;
    segmentData.SegmentTime = delta;
    segmentData.AddSegmentTime(delta);
    segmentData.AddSplitTime(elapsed);
    
    timer.LastSplitTime = elapsed;
    SegmentIndex++;
    if (SegmentIndex >= Segments.Count) {
      return false;
    }
    return true;
  }

  internal void Reset() {
    SegmentIndex = 0;
    var lastSplitTime = Segments[^1].SplitTime;
    bool isPB = false;
    if (lastSplitTime != null &&
    (lastSplitTime < PersonalBest || PersonalBest == null)) {
      isPB = true;
    }
    foreach (var seg in Segments) {
      if (seg.SplitTime == null) {
        continue;
      }
      if (isPB) {
        seg.PBSegmentTime = seg.SegmentTime;
        seg.PBSplitTime = seg.SplitTime;
      }
      seg.SegmentTime = seg.PBSegmentTime;
      seg.SplitTime = seg.PBSplitTime;
    }
    AttemptCount++;
  }
  
  internal void SkipBack() {
    Segments[SegmentIndex].SegmentTime = TimeSpan.Zero;
    Segments[SegmentIndex].SplitTime = TimeSpan.Zero;
    if (SegmentIndex > 0) {
      SegmentIndex--;
    }
  }
  internal void SkipForward() {
    Segments[SegmentIndex].SegmentTime = TimeSpan.Zero;
    Segments[SegmentIndex].SplitTime = TimeSpan.Zero;
    if (SegmentIndex < Segments.Count) {
      SegmentIndex++;
    }
  }
  
}
/// <summary>
/// This class defines a single segment of a greater run and holds display information and timing data.
/// </summary>
/// <param name="label"></param>
public class SegmentData(string label) : INotifyPropertyChanged {
  #region UI Stuff
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
  #endregion

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
}


