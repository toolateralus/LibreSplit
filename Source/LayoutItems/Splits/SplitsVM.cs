using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LibreSplit.Timing;

namespace LibreSplit;

public class SplitsVM : ViewModelBase {
  private SplitsLayout layoutItem = new();
  public SplitsLayout LayoutItem {
    get => layoutItem;
    set {
      layoutItem = value;
      OnPropertyChanged();
    }
  }
  readonly LibreSplitContext ctx;
  public SegmentData? activeSegment;

  public SplitsVM() {
    ctx = MainWindow.GlobalContext;
    ctx.PropertyChanged += (s, e) => {
      if (e.PropertyName != null &&
          CtxChangedActions.TryGetValue(e.PropertyName, out Action<SplitsVM>? action)) {
        action.Invoke(this);
      }
    };
    foreach (var action in CtxChangedActions.Values) {
      action.Invoke(this);
    }
  }

  private Dictionary<string, Action<SplitsVM>> CtxChangedActions { get; } = new() {
    [nameof(ctx.Run)] = (o) => {
      o.SyncSegmentVMs();
      o.ctx.Run.Segments.CollectionChanged += (s, e) => {
        o.SyncSegmentVMs();
      };
    },
    [nameof(ctx.ActiveSegment)] = (o) => {
      if (o.activeSegment != null) {
        o.SegmentToVMs[o.activeSegment].IsActive = false;
      }
      o.activeSegment = o.ctx.ActiveSegment;
      if (o.activeSegment != null) {
        o.SegmentToVMs[o.activeSegment].IsActive = true;
      }
    },
  };

  public ObservableCollection<SegmentVM> SegmentVMs { get; } = [];
  public Dictionary<SegmentData, SegmentVM> SegmentToVMs { get; } = [];


  private void SyncSegmentVMs() {
    SegmentToVMs.Clear();
    SegmentVMs.Clear();
    foreach (var segment in ctx.Run.Segments) {
      SegmentVM segVM = new(segment);
      SegmentToVMs.Add(segment, segVM);
      SegmentVMs.Add(segVM);
    }
    OnPropertyChanged(nameof(SegmentVMs));
  }
}