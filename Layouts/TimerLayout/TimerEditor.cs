using Avalonia.Controls;

namespace LibreSplit.Layouts.TimerLayout;
public partial class TimerEditor : UserControl {
  public TimerEditor(TimerLayoutData layoutItem) {
    LayoutItem = layoutItem;
    DataContext = this;
    InitializeComponent();
  }
  public TimerLayoutData LayoutItem { get; set; }
}
