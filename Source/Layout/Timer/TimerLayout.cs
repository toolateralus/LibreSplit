using Avalonia.Controls;
using LibreSplit.ObjectModel;

namespace LibreSplit;

public class TimerLayout : LayoutItem {
  private string inactiveColor = "White";
  private string aheadColor = "Green";
  private string behindColor = "Red";

  public string InactiveColor {
    get => inactiveColor; set {
      inactiveColor = value;
      OnPropertyChanged();
    }
  }
  public string AheadColor {
    get => aheadColor; set {
      aheadColor = value;
      OnPropertyChanged();
    }
  }
  public string BehindColor {
    get => behindColor; set {
      behindColor = value;
      OnPropertyChanged();
    }
  }
  public override Control? Control => new TimerControl(this);
  public override Control? Editor => new TimerEditor(this);
  public override string LayoutItemName => "Timer";
}