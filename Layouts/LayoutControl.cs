using Avalonia.Controls;

namespace LibreSplit.Layouts;

public abstract class LayoutControl : UserControl {
  public abstract string HeightWeight { get; }
}