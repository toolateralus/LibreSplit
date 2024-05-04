using Avalonia.Controls;

namespace LibreSplit;

public class TimerLayout : LayoutItem {
  string InactiveColor {get; set;} = "White";
  string AheadColor {get; set;} = "Green";
  string BehindColor {get; set;} = "Red";
  public override Control? Control => new TimerControl() { LayoutItem = this };
}