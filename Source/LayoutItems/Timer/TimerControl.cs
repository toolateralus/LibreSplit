using Avalonia.Controls;

namespace LibreSplit;
public partial class TimerControl : UserControl {
  TimerVM viewModel = new();
  public TimerControl() {
    DataContext = viewModel;
    InitializeComponent();
  }

  public TimerLayout? LayoutItem { get; internal set; }
}