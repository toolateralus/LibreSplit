using Avalonia;

namespace LibreSplit;
public static class Setters {
  static Setters() {
    ClassesProperty.Changed.AddClassHandler<StyledElement>(HandlestringChanged);
  }
  public static readonly AttachedProperty<string> ClassesProperty =
    AvaloniaProperty.RegisterAttached<StyledElement, string>("Classes", typeof(Setters));

  private static void HandlestringChanged(StyledElement element, AvaloniaPropertyChangedEventArgs args) {
    if (args.NewValue is string value) {
      element.Classes.Clear();
      element.Classes.Add(value);
    }
  }
  public static void SetClasses(AvaloniaObject element, string value) {
    element.SetValue(ClassesProperty, value);
  }
  public static string GetClasses(AvaloniaObject element) {
    return element.GetValue(ClassesProperty);
  }
}
