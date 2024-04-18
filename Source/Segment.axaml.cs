using System;
using Avalonia.Markup.Xaml;
using LibreSplit.Timing;

namespace LibreSplit.Controls;
public partial class Segment : Avalonia.Controls.UserControl {
  public Segment() {
    InitializeComponent();
  }
  
  public void InitializeData(SegmentData data) {
    titleLabel.Content = data.Label;
    splitTimeLabel.Content = TimeSpan.Zero.ToString();
    segmentTimeLabel.Content = data.PersonalBest.ToString();
  }
  private void InitializeComponent() {
    AvaloniaXamlLoader.Load(this);
  }
}