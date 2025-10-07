using System.Collections.ObjectModel;
using LibreSplit.Layouts.SplitsLayout;
using LibreSplit.Layouts.TimerLayout;
namespace LibreSplit.Layouts;
public class LayoutData : ObservableCollection<LayoutItemData> {
  public static LayoutData Default { get; } = [
    new SplitsLayoutData(),
    new TimerLayoutData()
  ];
};
