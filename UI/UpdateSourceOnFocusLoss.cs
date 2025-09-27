using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using System;
using Avalonia.Data;

namespace LibreSplit.UI;
public class TextBoxTextUpdateSourceOnLostFocus : Behavior<TextBox> {
  static TextBoxTextUpdateSourceOnLostFocus() {
    TextProperty.Changed.Subscribe(e => {
      ((TextBoxTextUpdateSourceOnLostFocus)e.Sender).OnBindingValueChanged();
    });
  }
  public static readonly StyledProperty<string> TextProperty =
    AvaloniaProperty.Register<TextBoxTextUpdateSourceOnLostFocus, string>(
      name: "Text",
      defaultBindingMode: BindingMode.TwoWay
    );
  public string Text {
    get => GetValue(TextProperty);
    set => SetValue(TextProperty, value);
  }
  protected override void OnAttached() {
    if (AssociatedObject != null) {
      AssociatedObject.LostFocus += OnLostFocus;
    }
    base.OnAttached();
  }
  protected override void OnDetaching() {
    if (AssociatedObject != null) {
      AssociatedObject.LostFocus -= OnLostFocus;
    }
    base.OnDetaching();
  }
  private void OnLostFocus(object? sender, RoutedEventArgs e) {
    if (AssociatedObject?.Text != null) {
      Text = AssociatedObject.Text;
    }
  }
  private void OnBindingValueChanged() {
    if (AssociatedObject != null) {
      AssociatedObject.Text = Text;
    }
  }
}
