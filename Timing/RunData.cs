using System;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using LibreSplit.IO;
using Avalonia.Threading;

namespace LibreSplit.Timing;

/// <summary>
/// This class represents a group of segments which make up a splittable timeable 'Run'.
/// </summary>
[method: JsonConstructor]
public class RunData(ObservableCollection<SegmentData> segments) {
  [JsonIgnore]
  public Action? OnReset;

  /// <summary>
  /// The collection of segment data that defines this run.
  /// </summary>
  public ObservableCollection<SegmentData> Segments { get; } = segments;

  /// <summary>
  /// The best time achieved of an RTA attempt at these segments.
  /// </summary>
  [JsonIgnore]
  public TimeSpan? PersonalBest => Segments.LastOrDefault()?.PBSplitTime;

  /// <summary>
  /// Used to offset the start time, can start at a negative or positive time to adjust for certain speedrun conventions
  /// </summary>
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

  public TimeSpan? GetLastSplitTime() => SegmentIndex <= 0 ? TimeSpan.Zero : Segments[SegmentIndex - 1].SplitTime;

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

    var lastSplitTime = GetLastSplitTime();

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

  /// <summary>
  /// stop and reset current the run
  /// </summary>
  internal void Reset() {
    foreach (var seg in Segments) {
      seg.SegmentTime = null;
      seg.SplitTime = null;
    }
    SegmentIndex = 0;
    AttemptCount++;
    OnReset?.Invoke();
  }

  /// <summary>
  /// Write the times from the current run to the data.
  /// </summary>
  /// <param name="isPersonalBest"></param>
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

  public static RunData Default => new([SegmentData.Default]);
}


