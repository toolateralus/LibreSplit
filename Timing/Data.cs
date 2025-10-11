using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using LibreSplit.IO;
using Avalonia.Threading;

namespace LibreSplit.Timing;
/// <summary>
/// This class represents a group of segments which make up a splittable timeable 'Run'.
/// </summary>
public class RunData {
  public RunData() {
    Segments = [new()];
  }
  [JsonConstructor]
  public RunData(ObservableCollection<SegmentData> segments) {
    Segments = segments;
  }
  /// <summary>
  /// The collection of segment data that defines this run.
  /// </summary>
  public ObservableCollection<SegmentData> Segments { get; }
  /// <summary>
  /// The best time achieved of an RTA attempt at these segments.
  /// </summary>
  [JsonIgnore]
  public TimeSpan? PersonalBest => Segments.LastOrDefault()?.PBSplitTime;

  // for binding.
  public TimeSpan StartTime { get; set; } = TimeSpan.Zero;

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
    timer.Start(StartTime);
  }

  /// <summary>
  /// Advance to the next segment in the run and save current time data,
  /// </summary>
  /// <param name="timer"></param>
  /// <returns>True if the run still has remaining segments, false if the run is complete.</returns>
  public bool Split(Timer timer) {
    if (SegmentIndex > Segments.Count - 1) {
      Logs.LogError($"Failed to split: SegmentIndex ({SegmentIndex}) >= Segments.Count - 1 ({Segments.Count - 1})");
      return false;
    }

    SegmentData segmentData = Segments[SegmentIndex];
    TimeSpan elapsed = timer.Elapsed;

    TimeSpan? lastSplitTime;
    int lastSplitIndex = SegmentIndex - 1;

    if (lastSplitIndex < 0) {
      lastSplitTime = TimeSpan.Zero;
    }
    else {
      lastSplitTime = Segments[lastSplitIndex].SplitTime;
    }

    if (lastSplitTime is null) {
      segmentData.SegmentTime = null;
    }
    else {
      segmentData.SegmentTime = elapsed - lastSplitTime;
    }

    segmentData.SplitTime = elapsed;
    timer.LastSplitTime = elapsed;
    SegmentIndex++;

    if (SegmentIndex > Segments.Count - 1) {
      return false;
    }

    return true;
  }

  internal void Reset() {
    foreach (var seg in Segments) {
      seg.SegmentTime = null;
      seg.SplitTime = null;
    }
    SegmentIndex = 0;
    AttemptCount++;
    OnReset?.Invoke();
  }

  public void UpdateTimes(bool isPersonalBest) {
    foreach (var seg in Segments) {
      if (seg.SegmentTime is TimeSpan segmentTime) {
        seg.AddSegmentTime(segmentTime);
      }

      if (seg.SplitTime is TimeSpan splitTime) {
        seg.AddSplitTime(splitTime);
      }

      if (isPersonalBest) {
        seg.PBSegmentTime = seg.SegmentTime;
        seg.PBSplitTime = seg.SplitTime;
      }
    }
  }

  [JsonIgnore]
  public Action? OnReset;

  internal void SkipBack() {
    Segments[SegmentIndex].SegmentTime = null;
    Segments[SegmentIndex].SplitTime = null;

    if (SegmentIndex > 0) {
      SegmentIndex--;
    }
    else {
      SegmentIndex = 0;
    }
  }

  internal void SkipForward() {
    Segments[SegmentIndex].SegmentTime = null;
    Segments[SegmentIndex].SplitTime = null;

    if (SegmentIndex < Segments.Count - 1) {
      SegmentIndex++;
    }
    else {
      SegmentIndex = Segments.Count - 1;
    }
  }

}
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
}


