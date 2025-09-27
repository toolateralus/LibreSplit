using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace LibreSplit.Layouts.TimerLayout;
public partial class TimerControl : UserControl {
  TimerViewModel viewModel;
  public TimerControl(TimerLayoutData layoutItem) {
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
