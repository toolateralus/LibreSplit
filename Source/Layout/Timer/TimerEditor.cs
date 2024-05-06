using Avalonia.Controls;

namespace LibreSplit;
public partial class TimerEditor : UserControl {
  public TimerEditor(TimerLayout layoutItem) {
    LayoutItem = layoutItem;
    DataContext = this;
    InitializeComponent();
  }
  public TimerLayout LayoutItem {get; set;}
}