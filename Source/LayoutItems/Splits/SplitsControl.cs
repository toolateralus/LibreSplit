using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace LibreSplit;
public partial class SplitsControl : UserControl {
  SplitsVM viewModel = new();
  public SplitsControl() {
    DataContext = viewModel;
    InitializeComponent();
  }

  protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
    viewModel.OnAttachedToLogicalTree();
    base.OnAttachedToLogicalTree(e);
  }

  protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e) {
    viewModel.OnDetachedFromLogicalTree();
    base.OnDetachedFromLogicalTree(e);
  }

  public SplitsLayout LayoutItem {
    get => viewModel.LayoutItem;
    internal set => viewModel.LayoutItem = value;
  }
}