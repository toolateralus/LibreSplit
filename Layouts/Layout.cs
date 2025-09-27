using System.Collections.ObjectModel;
using LibreSplit.Layouts.SplitsLayout;
using LibreSplit.Layouts.TimerLayout;
namespace LibreSplit.Layouts;
public class Layout : ObservableCollection<LayoutData> {
  public static Layout Default { get; } = [
    new SplitsLayoutData(),
    new TimerLayoutData()
  ];
};
