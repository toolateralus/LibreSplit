using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace LibreSplit.Convert;
/// <summary>
/// Converts a System.TimeSpan to a string and back.
/// used for Binding to UI elements.
/// </summary>
public class TimeSpanToStringConverter : IValueConverter {
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is TimeSpan timeSpan) {
      double totalSeconds = Math.Floor(timeSpan.TotalSeconds);
      TimeSpan flooredTimeSpan = TimeSpan.FromSeconds(totalSeconds);
      return flooredTimeSpan.ToString(@"hh\:mm\:ss");
    }
    return string.Empty;
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is string str) {
      if (TimeSpan.TryParse(str, out var timeSpan)) {
        return timeSpan;
      } else {
        Console.WriteLine("Unable to parse TimeSpan from given parameter : Likely the splits editor.");
      }
    }
    return AvaloniaProperty.UnsetValue;
  }
}