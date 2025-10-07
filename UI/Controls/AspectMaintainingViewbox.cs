using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace LibreSplit.UI.Controls;

public class AspectMaintainingViewbox : Decorator {
  protected override Size MeasureOverride(Size availableSize) {
    if (Child is null) return new();
    Child.Measure(availableSize);

    var aspectRatio = availableSize.Width / availableSize.Height;
    if (!double.IsNormal(aspectRatio)) {
      aspectRatio = 1;
    }

    return new(Child.DesiredSize.Height * aspectRatio, Child.DesiredSize.Height);
  }

  protected override Size ArrangeOverride(Size finalSize) {
    if (Child is null) return new();

    double aspectRatio = finalSize.Width / finalSize.Height;
    if (!double.IsNormal(aspectRatio)) {
      aspectRatio = 1;
    }

    double targetWidth = Child.DesiredSize.Height * aspectRatio;
    double targetHeight = Child.DesiredSize.Height;

    Child.Arrange(new Rect(0, 0, targetWidth, targetHeight));

    double scaleX = finalSize.Width / targetWidth;
    double scaleY = finalSize.Height / targetHeight;

    if (!double.IsNormal(scaleX) || !double.IsNormal(scaleY)) {
      scaleX = scaleY = 1;
    }

    Child.RenderTransform = new ScaleTransform(scaleX, scaleY);
    Child.RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);

    return finalSize;
  }
}
