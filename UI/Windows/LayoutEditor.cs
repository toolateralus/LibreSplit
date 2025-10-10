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
        button.Tag is LayoutItemData item) {
      GlobalContext.LayoutData.Remove(item);
    }
  }
  private void AddLayoutItem_Clicked(object? sender, RoutedEventArgs e) {
    if (sender is MenuItem menuItem &&
        menuItem.Tag is Type layoutItemType &&
        Activator.CreateInstance(layoutItemType) is LayoutItemData layoutItem) {
      GlobalContext.LayoutData.Add(layoutItem);
    }
  }
  public void MoveItemUp_Clicked(object? sender, RoutedEventArgs e) {
    if (sender is Button button &&
        button.Tag is LayoutItemData item) {
      int currentIndex = GlobalContext.LayoutData.IndexOf(item);
      if (currentIndex > 0) {
        GlobalContext.LayoutData.Move(currentIndex, currentIndex - 1);
      }
    }
  }
  public void MoveItemDown_Clicked(object? sender, RoutedEventArgs e) {
    if (sender is Button button &&
        button.Tag is LayoutItemData item) {
      int currentIndex = GlobalContext.LayoutData.IndexOf(item);
      if (currentIndex < GlobalContext.LayoutData.Count - 1) {
        GlobalContext.LayoutData.Move(currentIndex, currentIndex + 1);
      }
    }
  }
  public List<MenuItem> LayoutItemTypes {
    get {
      List<MenuItem> layoutItemMenuItems = [];
      var assemblies = AppDomain.CurrentDomain.GetAssemblies();
      foreach (var assembly in assemblies) {
        IEnumerable<Type> types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(LayoutItemData)) && !t.IsAbstract);
        foreach (var type in types) {
          LayoutItemData? data = (LayoutItemData?)Activator.CreateInstance(type);

          string header = type.Name;
          if (data is not null) {
            header = data.LayoutItemName;
          }
          MenuItem item = new() { Header = header, Tag = type };
          item.Click += AddLayoutItem_Clicked;
          layoutItemMenuItems.Add(item);
        }
      }
      return layoutItemMenuItems;
    }
  }
  public static LibreSplitContext GlobalContext => MainWindow.GlobalContext;
}
