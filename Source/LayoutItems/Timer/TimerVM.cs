namespace LibreSplit;

public class TimerVM : ViewModelBase {
  private TimerLayout layoutItem = new();
  public TimerLayout LayoutItem {
    get => layoutItem;
    set {
      layoutItem = value;
      OnPropertyChanged();
    }
  }
  public LibreSplitContext DataContext { get; }
  public TimerVM() {
    DataContext = MainWindow.GlobalContext;
  }
}