using Avalonia.Controls;
using Avalonia.Media;

namespace LibreSplit.Layouts.SplitsLayout;
public partial class SplitsEditor : UserControl {
  public SplitsEditor(SplitsLayoutData layoutItem) {
    LayoutItem = layoutItem;
    DataContext = this;
    InitializeComponent();
  }
  public Color ActiveBGColor {
    get => Color.Parse(LayoutItem.ActiveBGColor);
    set {
      LayoutItem.ActiveBGColor = value.ToString();
    }
  }
  public Color InactiveBGColor {
    get => Color.Parse(LayoutItem.InactiveBGColor);
    set {
      LayoutItem.InactiveBGColor = value.ToString();
    }
  }
  public Color AheadGainingTime {
    get => Color.Parse(LayoutItem.AheadGainingTimeColor);
    set {
      LayoutItem.AheadGainingTimeColor = value.ToString();
    }
  }
  public Color AheadLosingTimeColor {
    get => Color.Parse(LayoutItem.AheadLosingTimeColor);
    set {
      LayoutItem.AheadLosingTimeColor = value.ToString();
    }
  }
  public Color BehindGainingTimeColor {
    get => Color.Parse(LayoutItem.BehindGainingTimeColor);
    set {
      LayoutItem.BehindGainingTimeColor = value.ToString();
    }
  }
  public Color BehindLosingTimeColor {
    get => Color.Parse(LayoutItem.BehindLosingTimeColor);
    set {
      LayoutItem.BehindLosingTimeColor = value.ToString();
    }
  }
  public SplitsLayoutData LayoutItem { get; internal set; }
}
