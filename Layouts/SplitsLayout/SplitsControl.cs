using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace LibreSplit.Layouts.SplitsLayout;
public partial class SplitsControl : UserControl {
  public SplitsViewModel ViewModel { get; set; }
  public SplitsControl(SplitsLayoutData layoutItem) {
    DataContext = ViewModel = new(layoutItem);
    InitializeComponent();
  }

  protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
    ViewModel.OnAttachedToLogicalTree();
    base.OnAttachedToLogicalTree(e);
  }

  protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e) {
    ViewModel.OnDetachedFromLogicalTree();
    base.OnDetachedFromLogicalTree(e);
  }
}
