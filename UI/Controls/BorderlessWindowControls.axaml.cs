using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using LibreSplit.IO;
namespace LibreSplit.UI.Controls;

public partial class BorderlessWindowControls : UserControl {
  public BorderlessWindowControls() {
    InitializeComponent();
  }

  private Window? ParentWindow => this.FindAncestorOfType<Window>();

  private void ResizeWindow(object? sender, PointerPressedEventArgs e) {
    if (sender is Border border && border.Tag is string tag) {
      try {
        var edge = Enum.Parse<WindowEdge>(tag);
        e.Handled = true;
        ParentWindow?.BeginResizeDrag(edge, e);
      }
      catch (Exception exception) {
        Logs.LogError(exception);
      }
    }
  }
  private void MoveWindow(object? sender, PointerPressedEventArgs e) {
    e.Handled = true;
    // Only start move drag if left mouse button is pressed
    if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) {
      ParentWindow?.BeginMoveDrag(e);
    }
  }
}