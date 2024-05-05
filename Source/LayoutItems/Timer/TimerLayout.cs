using Avalonia.Controls;

namespace LibreSplit;

public class TimerLayout : LayoutItem {
  public string InactiveColor {get; set;} = "White";
  public string AheadColor {get; set;} = "Green";
  public string BehindColor {get; set;} = "Red";
  public override Control? Control => new TimerControl() { LayoutItem = this };
}