using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace LibreSplit.UI;
/// <summary>
/// Converts a System.TimeSpan to a string and back.
/// used for Binding to UI elements.
/// </summary>
public class NullableTimeSpanToStringConverter : IValueConverter {
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
    bool signed = false;
    if (parameter is string paramString && paramString == "Signed") {
      signed = true;
    }
    return (value as TimeSpan?).ToFormattedString(signed);
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is string str) {
      if (str == string.Empty || str == "―") {
        return null;
      }
      bool isNegative = false;
      if (str[0] == '-') {
        isNegative = true;
        str = str[1..];
      }
      else if (str[0] == '+') {
        str = str[1..];
      }
      TimeSpan timeSpan = TimeSpan.Zero;
      string[] timeParts = str.Split(':');
      if (timeParts.Length > 0 && float.TryParse(timeParts[^1], out var seconds)) {
        timeSpan = timeSpan.Add(TimeSpan.FromSeconds(seconds));
      }
      else {
        return AvaloniaProperty.UnsetValue;
      }
      if (timeParts.Length > 1) {
        if (int.TryParse(timeParts[^2], out var minutes)) {
          timeSpan = timeSpan.Add(TimeSpan.FromMinutes(minutes));
        }
        else {
          return AvaloniaProperty.UnsetValue;
        }
      }
      if (timeParts.Length > 2) {
        if (int.TryParse(timeParts[^3], out var hours)) {
          timeSpan = timeSpan.Add(TimeSpan.FromHours(hours));
        }
        else {
          return AvaloniaProperty.UnsetValue;
        }
      }
      return isNegative ? -timeSpan : timeSpan;
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

public class StringToFontFamilyConverter : IValueConverter {
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is string fontFamilyName && !string.IsNullOrWhiteSpace(fontFamilyName)) {
      try {
        return new FontFamily(fontFamilyName);
      }
      catch {
        return FontFamily.Default;
      }
    }
    return FontFamily.Default;
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
    return value is FontFamily ff ? ff.Name : null;
  }
}
