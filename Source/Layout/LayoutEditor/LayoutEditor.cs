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
  public static LibreSplitContext GlobalContext => MainWindow.GlobalContext;
}