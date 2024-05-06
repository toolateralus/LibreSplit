using Avalonia.Controls;
using LibreSplit.ObjectModel;

namespace LibreSplit;

public class TimerLayout : LayoutItem {
  public Observable<string> InactiveColor {get; set;} = "White";
  public Observable<string> AheadColor {get; set;} = "Green";
  public Observable<string> BehindColor {get; set;} = "Red";
  public override Control? Control => new TimerControl() { LayoutItem = this };
  public override Control? Editor => new TimerEditor(this);
  public override string LayoutItemName => "Timer";
}