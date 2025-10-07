using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using LibreSplit.Timing;
using LibreSplit.UI;
using LibreSplit.UI.Windows;

namespace LibreSplit.Layouts.SplitsLayout;

public class SplitsViewModel : ViewModelBase {
  public SplitsLayoutData LayoutItem { get; }
  public ObservableCollection<SegmentViewModel> SegmentViewModels { get; } = [];

  private readonly Dictionary<SegmentData, SegmentViewModel> segmentToViewModels = [];
  private LibreSplitContext ctx = null!;
  private SegmentData? activeSegment;
  private RunData run = null!;
  private Timer? timer;
  private readonly Dictionary<string, Action> ctxChangedActions;
  private readonly List<Comparer> comparers;

  public SplitsViewModel(SplitsLayoutData layoutItem) {
    LayoutItem = layoutItem;
    comparers = [
      new("Best", ComparisonType.BestSegmentDelta, ComparisonType.BestSegment),
      new("+/-", ComparisonType.PBSplitDelta, ComparisonType.PBSegment),
      new("Split", ComparisonType.CurrentSplit, ComparisonType.PBSplit),
    ];
    ctxChangedActions = new() {
      [nameof(ctx.Run)] = () => {
        SyncSegmentViewModels();
        run.Segments.CollectionChanged -= RunCollectionChanged;
        run.OnReset -= OnReset;
        run = ctx.Run;
        run.Segments.CollectionChanged += RunCollectionChanged;
        run.OnReset += OnReset;
      },
      [nameof(ctx.ActiveSegment)] = () => {
        if (activeSegment != null) {
          if (ctx.Run.Segments.IndexOf(activeSegment) < ctx.Run.SegmentIndex) {
            segmentToViewModels[activeSegment].SetStatus(SegmentStatus.Passed, comparers);
          }
          else {
            segmentToViewModels[activeSegment].SetStatus(SegmentStatus.Upcoming, comparers);
          }
        }
        if (ctx.ActiveSegment is not null) {
          segmentToViewModels[ctx.ActiveSegment].SetStatus(SegmentStatus.Current, comparers);
        }
        activeSegment = ctx.ActiveSegment;
      },
    };
  }

  private void UpdateComparer(SegmentViewModel segmentViewModel) {
    segmentViewModel.IsActive = true;
    for (int i = 0; i < comparers.Count; i++) {
      var comparison = segmentViewModel.Comparisons[i];
      var comparer = comparers[i];
      comparer.Update(comparison, segmentViewModel.Segment, timer);
    }
  }

  public void OnAttachedToLogicalTree() {
    ctx = MainWindow.GlobalContext;
    run = ctx.Run;
    run.Segments.CollectionChanged += RunCollectionChanged;
    run.OnReset += OnReset;
    ctx.PropertyChanged += CtxPropertyChanged;
    timer = MainWindow.GlobalContext.Timer;
    timer.PropertyChanged += TimerPropertyChanged;
    foreach (var action in ctxChangedActions.Values) {
      action.Invoke();
    }
  }

  private void TimerPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    if (e.PropertyName == nameof(Timer.Elapsed) && ctx.ActiveSegment is not null) {
      UpdateComparer(segmentToViewModels[ctx.ActiveSegment]);
    }
  }

  private void OnReset() {
    foreach (var segmentViewModel in SegmentViewModels) {
      segmentViewModel.SetStatus(SegmentStatus.Upcoming, comparers);
    }
  }

  private void CtxPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    if (e.PropertyName != null && ctxChangedActions.TryGetValue(e.PropertyName, out Action? action)) {
      action.Invoke();
    }
  }

  public void OnDetachedFromLogicalTree() {
    ctx.PropertyChanged -= CtxPropertyChanged;
    run.Segments.CollectionChanged -= RunCollectionChanged;
    run.OnReset -= OnReset;
    run = null!;
    activeSegment = null;
    if (timer != null) {
      timer.PropertyChanged -= TimerPropertyChanged;
      timer = null;
    }
  }

  private void RunCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
    SyncSegmentViewModels();
  }

  private void SyncSegmentViewModels() {
    segmentToViewModels.Clear();
    SegmentViewModels.Clear();
    foreach (var segment in ctx.Run.Segments) {
      SegmentViewModel segViewModel = new(segment);
      for (int i = 0; i < comparers.Count; i++) {
        segViewModel.Comparisons.Add(new());
      }
      segViewModel.SetStatus(SegmentStatus.Upcoming, comparers);
      segmentToViewModels.Add(segment, segViewModel);
      SegmentViewModels.Add(segViewModel);
    }
    OnPropertyChanged(nameof(SegmentViewModels));
  }
}
