using Avalonia.Controls;

namespace LibreSplit;
public partial class SplitsControl : UserControl {
  SplitsVM viewModel = new();
  public SplitsControl() {
    DataContext = viewModel;
    InitializeComponent();
  }

  public SplitsLayout LayoutItem {
    get => viewModel.LayoutItem;
    internal set => viewModel.LayoutItem = value;
  }
}