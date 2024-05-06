using Avalonia.Controls;
using Avalonia.Media;

namespace LibreSplit;
public partial class SplitsEditor : UserControl {
  public SplitsEditor(SplitsLayout layoutItem) {
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
  public SplitsLayout LayoutItem { get; internal set; }
}