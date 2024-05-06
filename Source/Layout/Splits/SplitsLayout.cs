using Avalonia.Controls;
using LibreSplit.ObjectModel;

namespace LibreSplit;

public class SplitsLayout : LayoutItem {
  public Observable<string> ActiveBGColor { get; set; } = "SteelBlue";
  public Observable<string> InactiveBGColor { get; set; } = "Transparent";
  public override Control? Control => new SplitsControl() { LayoutItem = this };
  public override Control? Editor => new SplitsEditor(this);
  public override string LayoutItemName => "Splits";
}