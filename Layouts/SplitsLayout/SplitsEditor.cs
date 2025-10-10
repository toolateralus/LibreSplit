using Avalonia.Controls;

namespace LibreSplit.Layouts.SplitsLayout;
public partial class SplitsEditor : UserControl {
  public SplitsEditor(SplitsLayoutData layoutItem) {
    LayoutItem = layoutItem;
    DataContext = this;
    InitializeComponent();
  }
  public SplitsLayoutData LayoutItem { get; internal set; }
}
