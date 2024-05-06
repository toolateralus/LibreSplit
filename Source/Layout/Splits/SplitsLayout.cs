using Avalonia.Controls;
using LibreSplit.ObjectModel;

namespace LibreSplit;

public class SplitsLayout : LayoutItem {
  private string activeBGColor = "Purple";
  private string inactiveBGColor = "Transparent";

  public string ActiveBGColor {
    get => activeBGColor; set {
      activeBGColor = value;
      OnPropertyChanged();
    }
  }
  public string InactiveBGColor {
    get => inactiveBGColor; set {
      inactiveBGColor = value;
      OnPropertyChanged();
    }
  }
  public override Control? Control => new SplitsControl(this);
  public override Control? Editor => new SplitsEditor(this);
  public override string LayoutItemName => "Splits";
}