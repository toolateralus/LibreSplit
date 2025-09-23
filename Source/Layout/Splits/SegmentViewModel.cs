using LibreSplit.Timing;

namespace LibreSplit;

public class SegmentViewModel(SegmentData segmentData) : ViewModelBase {
  public SegmentData Segment { get; } = segmentData;
  private bool isActive;
  public bool IsActive {
    get => isActive;
    set {
      isActive = value;
      OnPropertyChanged();
    }
  }
}