using Avalonia.Controls;

namespace LibreSplit.Layouts.TimerLayout;

public class TimerLayoutData : LayoutData {
  private string inactiveColor = "White";
  private string aheadGainingTimeColor = "Green";
  private string aheadLosingTimeColor = "LightGreen";
  private string behindLosingTimeColor = "Red";
  private string behindGainingTimeColor = "LightCoral";

  public string InactiveColor {
    get => inactiveColor; set {
      inactiveColor = value;
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
  public string BehindLosingTimeColor {
    get => behindLosingTimeColor; set {
      behindLosingTimeColor = value;
      OnPropertyChanged();
    }
  }
  public string BehindGainingTimeColor {
    get => behindGainingTimeColor; set {
      behindGainingTimeColor = value;
      OnPropertyChanged();
    }
  }
  public override Control? Control => new TimerControl(this);
  public override Control? Editor => new TimerEditor(this);
  public override string LayoutItemName => "Timer";
}
