using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace LibreSplit;
public partial class TimerControl : UserControl {
  TimerVM viewModel = new();
  public TimerControl() {
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
  public TimerLayout? LayoutItem { get; internal set; }
}