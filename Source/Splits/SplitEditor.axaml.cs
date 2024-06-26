using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using LibreSplit.Timing;
using System.Windows.Input;
namespace LibreSplit.Controls;
public partial class SplitEditor : Window {
  public ICommand ClearFocus => new RelayCommand(_ => {
    FocusManager?.ClearFocus();
  });
  public object? SelectedItem {get; set;}
  public RunData Run { get; set; }
  public SplitEditor() {
    // This should never be used.
    // It is just here to get the compiler to stop complaining
    // with a warning.
    Run = null!;
    DataContext = this;
    throw new NotImplementedException("Do not use SplitEditor in your axaml code.");
  }
  public SplitEditor(RunData? run) {
    run ??= new();
    Run = run;
    DataContext = this;
    InitializeComponent();
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

  private void AddSplit() {
    Run.Segments.Add(new SegmentData("New Split"));
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
  }

  internal RunData? GetRun() {
    return Run;
  }
}