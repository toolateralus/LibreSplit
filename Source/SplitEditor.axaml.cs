using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using LibreSplit.Timing;
namespace LibreSplit.Controls;
public partial class SplitEditor : Window {
  private readonly RunData run;
  public SplitEditor() {
    // This should never be used.
    // It is just here to get the compiler to stop complaining
    // with a warning.
    run = null!;
    DataContext = run;
    throw new NotImplementedException("Do not use SplitEditor in your axaml code.");
  }
  public SplitEditor(RunData? run) {
    InitializeComponent();
    run ??= new();
    this.run = run;
    splitListBox.ItemsSource = run.Segments;
    startTimeBox.Text = run.StartTime.ToString();
    KeyDown += delegate(object? o, KeyEventArgs e) {
      if (!e.KeyModifiers.HasFlag(KeyModifiers.Control | KeyModifiers.Shift)) {
        return;        
      }
      
      switch(e.Key) {
        case Key.A: {
          AddSplit();
        } break;
        case Key.R: {
          RemoveSplit();
        } break;
      }
      
    };
    
  }
  public void OnAddSplitClicked(object sender, RoutedEventArgs e) {
    AddSplit();
  }
  
  private void AddSplit() {
    run.Segments.Add(new SegmentData("New Split"));
  }
  
  public void OnRemoveSplitClicked(object sender, RoutedEventArgs e) {
    RemoveSplit();
  }
  
  private void RemoveSplit() {
    // remove selected otherwise pop last.
    if (splitListBox.SelectedItem is SegmentData selectedSegment) {
      run.Segments.Remove(selectedSegment);
    } else {
      run.Segments.Remove(run.Segments.Last());
    }
  }

  internal RunData? GetRun() {
    return run;
  }
}