using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;

namespace LibreSplit.Timing;
public class RunData {
  public ObservableCollection<SegmentData> Segments { get; } = [];
  public TimeSpan PersonalBest { get; set; }
  public uint AttemptCount { get; set; } = 0;
  
  [JsonIgnore]
  public int SegmentIndex { get; private set; } = 0;
  public void Split(Timer timer) {
    Segments[SegmentIndex].AddSegmentTime(timer.Delta);
    Segments[SegmentIndex].AddSplitTime(timer.Elapsed);
    SegmentIndex++;
  }
}
public class SegmentData(string label, TimeSpan personalBest) {
  private readonly List<TimeSpan> SegmentTimeHistory = [];
  private readonly List<TimeSpan> SplitTimeHistory = [];
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


