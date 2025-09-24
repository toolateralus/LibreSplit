using System.Collections.Generic;
using System.Collections.ObjectModel;
using LibreSplit.Timing;

namespace LibreSplit;

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

  public ObservableCollection<ComparisonViewModel> Comparisons { get; set; } = [];

  public SegmentViewModel(SegmentData segmentData) {
    Segment = segmentData;
  }

  public void SetStatus(SegmentStatus status, List<Comparer> comparers) {
    IsActive = status == SegmentStatus.Current;
    for (int i = 0; i < comparers.Count; i++) {
      var comparison = Comparisons[i];
      var comparer = comparers[i];
      comparer.SetStatus(comparison, Segment, status);
    }
  }
}