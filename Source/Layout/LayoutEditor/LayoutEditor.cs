using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LibreSplit;
public partial class LayoutEditor : Window {
  public LayoutEditor() {
    DataContext = this;
    InitializeComponent();
  }
  public void RemoveItem_Clicked(object? sender, RoutedEventArgs e) {
    if (sender is Button button &&
        button.Tag is LayoutItem item) {
      GlobalContext.Layout.Remove(item);
    }
  }
  public void MoveItemUp_Clicked(object? sender, RoutedEventArgs e) {
    if (sender is Button button &&
        button.Tag is LayoutItem item) {
      int currentIndex = GlobalContext.Layout.IndexOf(item);
      if (currentIndex > 0) {
        GlobalContext.Layout.Move(currentIndex, currentIndex - 1);
      }
    }
  }
  public void MoveItemDown_Clicked(object? sender, RoutedEventArgs e) {
    if (sender is Button button &&
        button.Tag is LayoutItem item) {
      int currentIndex = GlobalContext.Layout.IndexOf(item);
      if (currentIndex < GlobalContext.Layout.Count - 1) {
        GlobalContext.Layout.Move(currentIndex, currentIndex + 1);
      }
    }
  }
  public static LibreSplitContext GlobalContext => MainWindow.GlobalContext;
}