using Avalonia.Controls;

namespace LibreSplit.Layouts.SplitsLayout;

public class SplitsLayoutData : LayoutData {
  private string activeBGColor = "Purple";
  private string inactiveBGColor = "Transparent";
  private string aheadGainingTimeColor = "Green";
  private string aheadLosingTimeColor = "LightGreen";
  private string behindGainingTimeColor = "IndianRed";
  private string behindLosingTimeColor = "Red";
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
  public string AheadGainingTimeColor {
    get => aheadGainingTimeColor; set {
      aheadGainingTimeColor = value;
      OnPropertyChanged();
    }
  }
  public string AheadLosingTimeColor {
    get => aheadLosingTimeColor; set {
      aheadLosingTimeColor = value;
      OnPropertyChanged();
    }
  }
  public string BehindGainingTimeColor {
    get => behindGainingTimeColor; set {
      behindGainingTimeColor = value;
      OnPropertyChanged();
    }
  }
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
