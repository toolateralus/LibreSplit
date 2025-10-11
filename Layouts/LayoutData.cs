using System.Collections.ObjectModel;
using LibreSplit.Layouts.SplitsLayout;
using LibreSplit.Layouts.TimerLayout;
using LibreSplit.UI;
namespace LibreSplit.Layouts;
public class LayoutData : ViewModelBase {
  private string backgroundColor = "Black";
  private string textColor = "White";
  private string aheadGainingTimeColor = "LimeGreen";
  private string aheadLosingTimeColor = "PaleGreen";
  private string behindGainingTimeColor = "IndianRed";
  private string behindLosingTimeColor = "Red";
  private string bestSegmentTimeColor = "Gold";
  private string font = "Adwaita Mono";

  public string BackgroundColor {
    get => backgroundColor;
    set {
      backgroundColor = value;
      OnPropertyChanged();
    }
  }
  public string TextColor {
    get => textColor;
    set {
      textColor = value;
      OnPropertyChanged();
    }
  }
  public string AheadGainingTimeColor {
    get => aheadGainingTimeColor;
    set {
      aheadGainingTimeColor = value;
      OnPropertyChanged();
    }
  }
  public string AheadLosingTimeColor {
    get => aheadLosingTimeColor;
    set {
      aheadLosingTimeColor = value;
      OnPropertyChanged();
    }
  }
  public string BehindGainingTimeColor {
    get => behindGainingTimeColor;
    set {
      behindGainingTimeColor = value;
      OnPropertyChanged();
    }
  }
  public string BehindLosingTimeColor {
    get => behindLosingTimeColor;
    set {
      behindLosingTimeColor = value;
      OnPropertyChanged();
    }
  }
  public string BestSegmentTimeColor {
    get => bestSegmentTimeColor;
    set {
      bestSegmentTimeColor = value;
      OnPropertyChanged();
    }
  }
  public string Font {
    get => font;
    set {
      font = value;
      OnPropertyChanged();
    }
  }


  public ObservableCollection<LayoutItemData> Items { get; set; } = [
    new SplitsLayoutData(),
    new TimerLayoutData(),
    new SumOfBest.LayoutItemData(),
  ];
};
