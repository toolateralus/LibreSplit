using Avalonia.Controls;

namespace LibreSplit;

public class SplitsLayout : LayoutItem {
  private string activeBGColor = "Purple";
  public string ActiveBGColor {
    get => activeBGColor; set {
      activeBGColor = value;
      OnPropertyChanged();
    }
  }

  private string inactiveBGColor = "Transparent";
  public string InactiveBGColor {
    get => inactiveBGColor; set {
      inactiveBGColor = value;
      OnPropertyChanged();
    }
  }
  private string aheadGainingTimeColor = "Green";
  public string AheadGainingTimeColor {
    get => aheadGainingTimeColor; set {
      aheadGainingTimeColor = value;
      OnPropertyChanged();
    }
  }
  private string aheadLosingTimeColor = "LightGreen";
  public string AheadLosingTimeColor {
    get => aheadLosingTimeColor; set {
      aheadLosingTimeColor = value;
      OnPropertyChanged();
    }
  }
  private string behindGainingTimeColor = "IndianRed";
  public string BehindGainingTimeColor {
    get => behindGainingTimeColor; set {
      behindGainingTimeColor = value;
      OnPropertyChanged();
    }
  }
  private string behindLosingTimeColor = "Red";
  public string BehindLosingTimeColor {
    get => behindLosingTimeColor; set {
      behindLosingTimeColor = value;
      OnPropertyChanged();
    }
  }

  public override Control? Control => new SplitsControl(this);
  public override Control? Editor => new SplitsEditor(this);
  public override string LayoutItemName => "Splits";
}