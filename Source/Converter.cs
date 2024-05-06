using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace LibreSplit.Convert;
/// <summary>
/// Converts a System.TimeSpan to a string and back.
/// used for Binding to UI elements.
/// </summary>
public class TimeSpanToStringConverter : IValueConverter {
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is TimeSpan timeSpan) {
      double totalms = Math.Floor(timeSpan.TotalMilliseconds);
      TimeSpan flooredTimeSpan = TimeSpan.FromMilliseconds(totalms);

      if (flooredTimeSpan.TotalMinutes < 1) {
        return flooredTimeSpan.ToString(@"s\.ff");
      }
      else if (flooredTimeSpan.TotalHours < 1) {
        return flooredTimeSpan.ToString(@"m\:ss\.ff");
      }
      else {
        return flooredTimeSpan.ToString(@"h\:mm\:ss\.ff");
      }
    }
    return string.Empty;
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is string str) {
      if (TimeSpan.TryParse(str, out var timeSpan)) {
        return timeSpan;
      }
      else {
        Console.WriteLine("Unable to parse TimeSpan from given parameter : Likely the splits editor.");
      }
    }
    return AvaloniaProperty.UnsetValue;
  }
}
public class StringToColorConverter : IValueConverter {
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is string colorString &&
        Color.TryParse(colorString, out var color)) {
      return color;
    }
    return AvaloniaProperty.UnsetValue;
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is Color color) {
      return color.ToString();
    }
    return AvaloniaProperty.UnsetValue;
  }
}