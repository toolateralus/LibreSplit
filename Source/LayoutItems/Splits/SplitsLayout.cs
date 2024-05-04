using Avalonia.Controls;

namespace LibreSplit;

public class SplitsLayout : LayoutItem {
  public string ActiveBGColor { get; set; } = "SteelBlue";
  public string InactiveBGColor { get; set; } = "Transparent";
  public override Control? Control => new SplitsControl() { LayoutItem = this };
}