using Avalonia.Controls;

namespace LibreSplit;
public partial class TimerControl : UserControl {
    public TimerControl(MainWindowVM vm) {
        DataContext = vm;
        InitializeComponent();
    }
}