using Avalonia.Controls;

namespace LibreSplit.Layouts.SplitsLayout;

public class SplitsLayoutData : LayoutItemData {
  private string activeSplitColor = "DodgerBlue";
  private string inactiveSplitColor = "Transparent";
  public string ActiveSplitColor {
    get => activeSplitColor; set {
      activeSplitColor = value;
      OnPropertyChanged();
    }
  }

  public string InactiveSplitColor {
    get => inactiveSplitColor; set {
      inactiveSplitColor = value;
      OnPropertyChanged();
    }
  }

  public override Control? Control => new SplitsControl(this);
  public override Control? Editor => new SplitsEditor(this);
  public override string LayoutItemName => "Splits";
}
