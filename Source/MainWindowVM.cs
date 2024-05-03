using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Media;
using LibreSplit.Timing;

namespace LibreSplit;
public class MainWindowVM : ViewModelBase {
  private TimeSpan elapsed;

  public ObservableCollection<Control> Layout {get;} = [];
  public TimeSpan Elapsed {
    get => elapsed;
	set {
      elapsed = value;
	  OnPropertyChanged();
    }
  }
	private ObservableCollection<SegmentData> segments = [];
  public ObservableCollection<SegmentData> Segments {
    get => segments;
    set {
      segments = value;
	    OnPropertyChanged();
    }
  }

  public SolidColorBrush ActiveBGColor { get; set; } = new(Colors.SteelBlue);
  public SolidColorBrush InactiveBGColor { get; set; } = new(Colors.Transparent);
  private SegmentData? _activeSegment = null;

  public void SetActiveSegment(SegmentData segment) {
    if (_activeSegment is SegmentData activeSegment) {
      activeSegment.BackgroundColor = InactiveBGColor;
    }
    _activeSegment = segment;
    segment.BackgroundColor = ActiveBGColor;
  }
  public void ClearActiveSegment() {
    if (_activeSegment is SegmentData activeSegment) {
      activeSegment.BackgroundColor = InactiveBGColor;
      _activeSegment = null;
    }
  }
}