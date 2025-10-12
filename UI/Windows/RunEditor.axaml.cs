using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using LibreSplit.Timing;
using System.Windows.Input;
namespace LibreSplit.UI.Windows;
public partial class RunEditor : Window {
  public ICommand ClearFocus => new RelayCommand(_ => {
    FocusManager?.ClearFocus();
  });

  public object? SelectedItem { get; set; }
  public RunData Run { get; set; }
  public RunEditor() {
    // This should never be used.
    // It is just here to get the compiler to stop complaining
    // with a warning.
    Run = null!;
    DataContext = this;
    throw new NotImplementedException("Do not use RunEditor in your axaml code.");
  }
  public RunEditor(RunData? run) {
    run ??= new();
    Run = run;
    DataContext = this;
    InitializeComponent();
    Topmost = true;
    KeyDown += delegate (object? o, KeyEventArgs e) {
      if (!e.KeyModifiers.HasFlag(KeyModifiers.Control | KeyModifiers.Shift)) {
        return;
      }
      switch (e.Key) {
        case Key.A: {
            AddSplit();
          }
          break;
        case Key.R: {
            RemoveSplit();
          }
          break;
      }
    };

  }

  public void OnAddSplitClicked(object sender, RoutedEventArgs e) {
    AddSplit();
  }

  public void OnClearHistoryClicked(object sender, RoutedEventArgs e) {
    foreach (var split in Run.Segments) {
      split.SegmentTimeHistory.Clear();
    }
  }

  public void OnClearTimesClicked(object sender, RoutedEventArgs e) {
    foreach (var split in Run.Segments) {
      split.PBSplitTime = null;
      split.PBSegmentTime = null;
    }
    
  }

  private void AddSplit() {
    if (SelectedItem is SegmentData selectedSegment) {
      Run.Segments.Insert(Run.Segments.IndexOf(selectedSegment) + 1, new());
    }
    else {
      Run.Segments.Add(new());
    }
  }

  public void OnRemoveSplitClicked(object sender, RoutedEventArgs e) {
    RemoveSplit();
  }

  private void RemoveSplit() {
    // remove selected otherwise pop last.
    if (SelectedItem is SegmentData selectedSegment) {
      Run.Segments.Remove(selectedSegment);
    }
    else {
      Run.Segments.Remove(Run.Segments.Last());
    }
    if (Run.Segments.Count == 0) {
      AddSplit();
    }
  }

  internal RunData? GetRun() {
    return Run;
  }
}
