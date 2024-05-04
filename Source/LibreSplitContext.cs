using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using LibreSplit.Timing;

namespace LibreSplit;
public class LibreSplitContext : ViewModelBase {
  public ObservableCollection<LayoutItem> Layout {get;} = [];

  private TimeSpan elapsed;
  public TimeSpan Elapsed {
    get => elapsed;
	set {
      elapsed = value;
	  OnPropertyChanged();
    }
  }

	private RunData run = new();
  public RunData Run {
    get => run;
    set {
      run = value;
	    OnPropertyChanged();
    }
  }

  private SegmentData? activeSegment;
  public SegmentData? ActiveSegment {
    get => activeSegment;
    set {
      activeSegment = value;
      OnPropertyChanged();
    }
  }

  public void SetActiveSegment(SegmentData segment) {
    ActiveSegment = segment;
  }
  public void ClearActiveSegment() {
    ActiveSegment = null;
  }
}