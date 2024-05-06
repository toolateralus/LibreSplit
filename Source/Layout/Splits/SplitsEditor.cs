using Avalonia.Controls;

namespace LibreSplit;
public partial class SplitsEditor : UserControl {
  public SplitsEditor(SplitsLayout layoutItem) {
    LayoutItem = layoutItem;
    DataContext = this;
    InitializeComponent();
  }
  public SplitsLayout LayoutItem { get; internal set; }
}