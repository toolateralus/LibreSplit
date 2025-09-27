using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace LibreSplit.Layouts.SplitsLayout;
public partial class SplitsControl : UserControl {
  SplitsViewModel viewModel;
  public SplitsControl(SplitsLayoutData layoutItem) {
    DataContext = viewModel = new(layoutItem);
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
}
