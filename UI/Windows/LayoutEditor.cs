using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using LibreSplit.Layouts;
using LibreSplit.Timing;

namespace LibreSplit.UI.Windows;
public partial class LayoutEditor : Window {
  public LayoutEditor() {
    DataContext = this;
    InitializeComponent();
  }
  public void RemoveItem_Clicked(object? sender, RoutedEventArgs e) {
    if (sender is Button button &&
        button.Tag is LayoutData item) {
      GlobalContext.Layout.Remove(item);
    }
  }
  private void AddLayoutItem_Clicked(object? sender, RoutedEventArgs e) {
    if (sender is MenuItem menuItem &&
        menuItem.Tag is Type layoutItemType &&
        Activator.CreateInstance(layoutItemType) is LayoutData layoutItem) {
      GlobalContext.Layout.Add(layoutItem);
    }
  }
  public void MoveItemUp_Clicked(object? sender, RoutedEventArgs e) {
    if (sender is Button button &&
        button.Tag is LayoutData item) {
      int currentIndex = GlobalContext.Layout.IndexOf(item);
      if (currentIndex > 0) {
        GlobalContext.Layout.Move(currentIndex, currentIndex - 1);
      }
    }
  }
  public void MoveItemDown_Clicked(object? sender, RoutedEventArgs e) {
    if (sender is Button button &&
        button.Tag is LayoutData item) {
      int currentIndex = GlobalContext.Layout.IndexOf(item);
      if (currentIndex < GlobalContext.Layout.Count - 1) {
        GlobalContext.Layout.Move(currentIndex, currentIndex + 1);
      }
    }
  }
  public List<MenuItem> LayoutItemTypes {
    get {
      List<MenuItem> layoutItemTypes = [];
      var assemblies = AppDomain.CurrentDomain.GetAssemblies();
      foreach (var assembly in assemblies) {
        IEnumerable<Type> types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(LayoutData)) && !t.IsAbstract);
        foreach (var type in types) {
          var item = new MenuItem() { Header = type.Name, Tag = type };
          item.Click += AddLayoutItem_Clicked;
          layoutItemTypes.Add(item);
        }
      }
      return layoutItemTypes;
    }
  }
  public static LibreSplitContext GlobalContext => MainWindow.GlobalContext;
}
