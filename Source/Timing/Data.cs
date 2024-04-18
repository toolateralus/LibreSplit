using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;

namespace LibreSplit.Timing;
/// <summary>
/// This class represents a group of segments which make up a splittable timeable 'Run'.
/// </summary>
public class RunData {
  /// <summary>
  /// The collection of segment data that defines this run.
  /// </summary>
  public ObservableCollection<SegmentData> Segments { get; } = [];
  /// <summary>
  /// The best time achieved of an RTA attempt at these segments.
  /// </summary>
  public TimeSpan PersonalBest { get; set; }
  /// <summary>
  /// The total count of attempts made at this run.
  /// </summary>
  public uint AttemptCount { get; set; } = 0;
  
  /// <summary>
  /// The current segment index of an in-progress run.
  /// </summary>
  [JsonIgnore]
  public int SegmentIndex { get; private set; } = 0;
  
  /// <summary>
  /// Advance to the next segment in the run and save current time data,
  /// </summary>
  /// <param name="timer"></param>
  /// <returns>True if the run still has remaining segments, false if the run is complete.</returns>
  public bool Split(Timer timer) {
    if (SegmentIndex >= Segments.Count) {
      return false;
    }
    Segments[SegmentIndex].AddSegmentTime(timer.Delta);
    Segments[SegmentIndex].AddSplitTime(timer.Elapsed);
    timer.LastSplitTime = timer.Elapsed;
    SegmentIndex++;
    return true;
  }
  
  internal void Reset() {
    SegmentIndex = 0;
    foreach (var seg in Segments) {
      seg.CurrentDelta = seg.PersonalBest;
      seg.CurrentSplitTime = seg.BestSplitTime();
    }
    AttemptCount++;
  }
}
/// <summary>
/// This class defines a single segment of a greater run and holds display information and timing data.
/// </summary>
/// <param name="label"></param>
/// <param name="personalBest"></param>
public class SegmentData(string label, TimeSpan personalBest) : INotifyPropertyChanged {
  #region UI Stuff
  private TimeSpan _currentDelta;
  public TimeSpan CurrentDelta {
    get { return _currentDelta; }
    set {
      if (_currentDelta != value) {
        _currentDelta = value;
        OnPropertyChanged();
      }
    }
  }
  
  private TimeSpan _currentSplitTime;
  public TimeSpan CurrentSplitTime {
    get { return _currentSplitTime; }
    set {
      if (_currentSplitTime != value) {
        _currentSplitTime = value;
        OnPropertyChanged();
      }
    }
  }
  public event PropertyChangedEventHandler? PropertyChanged;
  
  protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  #endregion
  
  public List<TimeSpan> SegmentTimeHistory {get;set;}= [];
  public List<TimeSpan> SplitTimeHistory {get;set;} = [];
  public string Label { get; set; } = label;
  
  public TimeSpan PersonalBest {
    get;
  } = personalBest;
  public void AddSegmentTime(TimeSpan delta) {
    SplitTimeHistory.Add(delta);
  }
  public void AddSplitTime(TimeSpan time) {
    SplitTimeHistory.Add(time);
  }

  public TimeSpan BestSegmentTime() {
    return SegmentTimeHistory.Min();
  }
  public TimeSpan WorstSegmentTime() {
    return SegmentTimeHistory.Max();
  }
  public TimeSpan AverageSegmentTime() {
    long averageTicks = (long)SegmentTimeHistory.Average(timeSpan => timeSpan.Ticks);
    return new TimeSpan(averageTicks);
  }
  public TimeSpan BestSplitTime() {
    return SplitTimeHistory.Min();
  }
  public TimeSpan WorstSplitTime() {
    return SplitTimeHistory.Max();
  }
  public TimeSpan AverageSplitTime() {
    long averageTicks = (long)SplitTimeHistory.Average(timeSpan => timeSpan.Ticks);
    return new TimeSpan(averageTicks);
  }
}


