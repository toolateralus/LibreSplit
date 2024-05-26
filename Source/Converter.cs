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
public class NullableTimeSpanToStringConverter : IValueConverter {
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
      else if (flooredTimeSpan.TotalDays < 1) {
        return flooredTimeSpan.ToString(@"h\:mm\:ss\.ff");
      } else {
        var totalHours = flooredTimeSpan.Hours + (flooredTimeSpan.Days * 24);
        string minutesSeconds = flooredTimeSpan.ToString(@"mm\:ss\.ff");
        return totalHours.ToString() + ":" + minutesSeconds;
      }
    }
    return "―";
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is string str) {
      if (str == string.Empty || str == "―") {
        return null;
      }
      TimeSpan timeSpan = TimeSpan.Zero;
      string[] timeParts = str.Split(':');
      if (timeParts.Length > 0 && float.TryParse(timeParts[^1], out var seconds)) {
        timeSpan = timeSpan.Add(TimeSpan.FromSeconds(seconds));
      } else {
        return AvaloniaProperty.UnsetValue;
      }
      if (timeParts.Length > 1) {
        if (int.TryParse(timeParts[^2], out var minutes)) {
          timeSpan = timeSpan.Add(TimeSpan.FromMinutes(minutes));
        } else {
          return AvaloniaProperty.UnsetValue;
        }
      }
      if (timeParts.Length > 2) {
        if (int.TryParse(timeParts[^3], out var hours)) {
          timeSpan = timeSpan.Add(TimeSpan.FromHours(hours));
        } else {
          return AvaloniaProperty.UnsetValue;
        }
      }
      return timeSpan;
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