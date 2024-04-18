using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace LibreSplit.Convert;
public class TimeSpanToStringConverter : IValueConverter {
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is TimeSpan timeSpan) {
      return timeSpan.ToString();
    }
    return AvaloniaProperty.UnsetValue;
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is string str) {
      if (TimeSpan.TryParse(str, out var timeSpan)) {
        return timeSpan;
      }
    }
    return AvaloniaProperty.UnsetValue;
  }
}