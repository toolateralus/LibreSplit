using Avalonia.Controls;

namespace LibreSplit.Layouts.TimerLayout;

public class TimerLayoutData : LayoutItemData {
  private string backgroundColor = "Transparent";

  public string BackgroundColor {
    get => backgroundColor; set {
      backgroundColor = value;
      OnPropertyChanged();
    }
  }
  public override Control? Control => new TimerControl(this);
  public override Control? Editor => new TimerEditor(this);
  public override string LayoutItemName => "Timer";
}
