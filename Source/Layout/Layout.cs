using System.Collections.ObjectModel;
namespace LibreSplit;
public class Layout : ObservableCollection<LayoutItem> {
  public static Layout Default { get;} = [
    new SplitsLayout(),
    new TimerLayout()
  ];
};