using System;
using LibreSplit.Timing;

namespace LibreSplit;

public enum ComparisonType {
  None,
  CurrentSegment,
  CurrentSplit,
  PBSegment,
  PBSplit,
  PBSegmentDelta,
  PBSplitDelta,
}

public record Comparer(string Label, ComparisonType PassedComparison, ComparisonType UpcomingComparison) {
  public void SetStatus(ComparisonViewModel comparison, SegmentData segment, SegmentStatus status) {
    if (status == SegmentStatus.Current) {
      comparison.Classes = "Normal";
      return;
    }
    var comparisonType = status switch {
      SegmentStatus.Passed => PassedComparison,
      SegmentStatus.Upcoming => UpcomingComparison,
      _ => UpcomingComparison,
    };
    switch (comparisonType) {
      case ComparisonType.CurrentSegment:
        comparison.Signed = false;
        comparison.Time = segment.SegmentTime;
        break;
      case ComparisonType.CurrentSplit:
        comparison.Signed = false;
        comparison.Time = segment.SplitTime;
        break;
      case ComparisonType.PBSegment:
        comparison.Signed = false;
        comparison.Time = segment.PBSegmentTime;
        break;
      case ComparisonType.PBSplit:
        comparison.Signed = false;
        comparison.Time = segment.PBSplitTime;
        break;
      case ComparisonType.PBSegmentDelta:
        comparison.Time = segment.SegmentTime - segment.PBSegmentTime;
        comparison.Signed = true;
        comparison.Classes = DeltaColor(segment.SplitTime, segment.SegmentTime, segment.PBSplitTime, segment.PBSegmentTime);
        break;
      case ComparisonType.PBSplitDelta:
        comparison.Time = segment.SplitTime - segment.PBSplitTime;
        comparison.Signed = true;
        comparison.Classes = DeltaColor(segment.SplitTime, segment.SegmentTime, segment.PBSplitTime, segment.PBSegmentTime);
        break;
      case ComparisonType.None:
        comparison.Time = null;
        break;
    }
  }
  string DeltaColor(TimeSpan? split, TimeSpan? segment, TimeSpan? comparisonSplit, TimeSpan? comparisonSegment) {
    if (split is null || comparisonSegment is null) {
      return "Normal";
    } else if (split < comparisonSplit) {
      if (comparisonSegment is null || segment < comparisonSegment) {
        return "AheadGainingTime";
      }
      else {
        return "AheadLosingTime";
      }
    }
    else {
      if (comparisonSegment is null || segment < comparisonSegment) {
        return "BehindGainingTime";
      }
      else {
        return "BehindLosingTime";
      }
    }
  }
  public void Update(ComparisonViewModel comparison, SegmentData segment, Timer? timer) {
    switch (PassedComparison) {
      case ComparisonType.PBSegmentDelta:
        if (timer is null) {
          return;
        }
        comparison.Signed = true;
        comparison.Time = timer.Delta - segment.PBSegmentTime;
        break;
      case ComparisonType.PBSplitDelta:
        if (timer is null) {
          return;
        }
        comparison.Signed = true;
        comparison.Time = timer.Elapsed - segment.PBSplitTime;
        break;
      default:
        break;
    }
  }
}