namespace LibreSplit.Layouts.SumOfBest;

public class LayoutItemData : Layouts.LayoutItemData {
  public override Avalonia.Controls.Control? Control => new Control(new ViewModel());
  public override Avalonia.Controls.Control? Editor => new();
  public override string LayoutItemName => "Sum of Best Segements";
}