using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using LibreSplit.Timing;

namespace LibreSplit;

public class SplitsViewModel : ViewModelBase {
  public SplitsViewModel(SplitsLayout layoutItem) {
    LayoutItem = layoutItem;
  }
  public SplitsLayout LayoutItem {get;}
  
  LibreSplitContext ctx = null!;
  public SegmentData? activeSegment;

  public void OnAttachedToLogicalTree() {
    ctx = MainWindow.GlobalContext;
    ctx.PropertyChanged += CtxPropertyChanged;
    foreach (var action in CtxChangedActions.Values) {
      action.Invoke(this);
    }
  }

  private void CtxPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    if (e.PropertyName != null &&
        CtxChangedActions.TryGetValue(e.PropertyName, out Action<SplitsViewModel>? action)) {
      action.Invoke(this);
    }
  }

  public void OnDetachedFromLogicalTree() {
    ctx.PropertyChanged -= CtxPropertyChanged;
    foreach (var action in CtxChangedActions.Values) {
      action.Invoke(this);
    }
  }

  private Dictionary<string, Action<SplitsViewModel>> CtxChangedActions { get; } = new() {
    [nameof(ctx.Run)] = (o) => {
      o.SyncSegmentViewModels();
      o.ctx.Run.Segments.CollectionChanged += o.RunCollectionChanged;
    },
    [nameof(ctx.ActiveSegment)] = (o) => {
      if (o.activeSegment != null) {
        o.SegmentToViewModels[o.activeSegment].IsActive = false;
      }
      o.activeSegment = o.ctx.ActiveSegment;
      if (o.activeSegment != null) {
        o.SegmentToViewModels[o.activeSegment].IsActive = true;
      }
    },
  };

  private void RunCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
    SyncSegmentViewModels();
  }

  public ObservableCollection<SegmentViewModel> SegmentViewModels { get; } = [];
  public Dictionary<SegmentData, SegmentViewModel> SegmentToViewModels { get; } = [];


  private void SyncSegmentViewModels() {
    SegmentToViewModels.Clear();
    SegmentViewModels.Clear();
    foreach (var segment in ctx.Run.Segments) {
      SegmentViewModel segViewModel = new(segment);
      SegmentToViewModels.Add(segment, segViewModel);
      SegmentViewModels.Add(segViewModel);
    }
    OnPropertyChanged(nameof(SegmentViewModels));
  }
}