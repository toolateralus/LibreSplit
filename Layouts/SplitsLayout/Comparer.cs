using System;
using LibreSplit.Timing;

namespace LibreSplit.Layouts.SplitsLayout;

public enum ComparisonType {
  None,
  CurrentSegment,
  CurrentSplit,
  PBSegment,
  PBSplit,
  PBSegmentDelta,
  PBSplitDelta,
  BestSegment,
  BestSplit,
  BestSegmentDelta,
  BestSplitDelta,
}

public record Comparer(string Label, ComparisonType PassedComparison, ComparisonType UpcomingComparison) {
  public void SetStatus(ComparisonViewModel comparison, SegmentData segment, SegmentStatus status) {
    if (status == SegmentStatus.Current) {
      comparison.Classes = "";
      return;
    }
    var comparisonType = status switch {
      SegmentStatus.Passed => PassedComparison,
      SegmentStatus.Upcoming => UpcomingComparison,
      _ => UpcomingComparison,
    };
    switch (comparisonType) {
      case ComparisonType.None:
        comparison.Time = null;
        break;
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
      case ComparisonType.BestSegment:
        comparison.Signed = false;
        comparison.Time = segment.BestSegmentTime();
        break;
      case ComparisonType.BestSplit:
        comparison.Signed = false;
        comparison.Time = segment.BestSplitTime();
        break;
      case ComparisonType.BestSegmentDelta:
        comparison.Time = segment.SegmentTime - segment.BestSegmentTime();
        comparison.Signed = true;
        if (segment.SplitTime is null || segment.SegmentTime is null) {
          comparison.Classes = "";
        }
        else if (segment.BestSegmentTime() is null) {
          comparison.Classes = "BestSegmentTime";
        }
        else if (segment.BestSegmentTime() > segment.SegmentTime) {
          comparison.Classes = "BestSegmentTime";
        }
        else {
          comparison.Classes = "BehindLosingTime";
        }
        break;
      case ComparisonType.BestSplitDelta:
        comparison.Time = segment.SplitTime - segment.BestSplitTime();
        comparison.Signed = true;
        comparison.Classes = DeltaColor(segment.SplitTime, segment.SegmentTime, segment.BestSplitTime(), segment.BestSegmentTime());
        break;
    }
    if (status == SegmentStatus.Upcoming) {
      comparison.Classes = "";
    }
  }
  private static string DeltaColor(TimeSpan? split, TimeSpan? segment, TimeSpan? comparisonSplit, TimeSpan? comparisonSegment) {
    if (split is null || comparisonSegment is null) {
      return "";
    }
    else if (split < comparisonSplit) {
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
      case ComparisonType.BestSegmentDelta:
        if (timer is null) {
          return;
        }
        comparison.Signed = true;
        comparison.Time = timer.Delta - segment.BestSegmentTime();
        break;
      case ComparisonType.BestSplitDelta:
        if (timer is null) {
          return;
        }
        comparison.Signed = true;
        comparison.Time = timer.Elapsed - segment.BestSplitTime();
        break;
      default:
        break;
    }
  }
}
