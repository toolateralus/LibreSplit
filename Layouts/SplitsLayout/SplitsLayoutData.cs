using Avalonia.Controls;

namespace LibreSplit.Layouts.SplitsLayout;

public class SplitsLayoutData : LayoutData {
  private string textColor = "White";
  private string activeSplitColor = "Purple";
  private string inactiveSplitColor = "Black";
  private string aheadGainingTimeColor = "Green";
  private string aheadLosingTimeColor = "LightGreen";
  private string behindGainingTimeColor = "IndianRed";
  private string behindLosingTimeColor = "Red";
  public string TextColor {
    get => textColor; set {
      textColor = value;
      OnPropertyChanged();
    }
  }
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
