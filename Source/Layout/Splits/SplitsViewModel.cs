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
    CtxChangedActions = new() {
      [nameof(ctx.Run)] = () => {
        SyncSegmentViewModels();
        run.Segments.CollectionChanged -= RunCollectionChanged;
        run = ctx.Run;
        run.Segments.CollectionChanged += RunCollectionChanged;
      },
      [nameof(ctx.ActiveSegment)] = () => {
        if (activeSegment != null) {
          if (ctx.Run.Segments.IndexOf(activeSegment) < ctx.Run.SegmentIndex) {
            SegmentToViewModels[activeSegment].SetStatus(SegmentStatus.Passed);
          }
          else {
            SegmentToViewModels[activeSegment].SetStatus(SegmentStatus.Upcoming);
          }
        }
        if (ctx.ActiveSegment != null) {
          SegmentToViewModels[ctx.ActiveSegment].SetStatus(SegmentStatus.Current);
        }
        activeSegment = ctx.ActiveSegment;
      },
    };
  }
  public SplitsLayout LayoutItem { get; }

  LibreSplitContext ctx = null!;
  private SegmentData? activeSegment;
  private RunData run = null!;

  public void OnAttachedToLogicalTree() {
    ctx = MainWindow.GlobalContext;
    run = ctx.Run;
    run.Segments.CollectionChanged += RunCollectionChanged;
    run.OnReset += OnReset;
    ctx.PropertyChanged += CtxPropertyChanged;
    foreach (var action in CtxChangedActions.Values) {
      action.Invoke();
    }
  }

  private void OnReset() {
    foreach (var segmentVM in SegmentViewModels) {
      segmentVM.SetStatus(SegmentStatus.Upcoming);
    }
  }

  private void CtxPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    if (e.PropertyName != null && CtxChangedActions.TryGetValue(e.PropertyName, out Action? action)) {
      action.Invoke();
    }
  }

  public void OnDetachedFromLogicalTree() {
    ctx.PropertyChanged -= CtxPropertyChanged;
    run.Segments.CollectionChanged -= RunCollectionChanged;
    run.OnReset -= OnReset;
    run = null!;
    activeSegment = null;
  }

  private Dictionary<string, Action> CtxChangedActions { get; }

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