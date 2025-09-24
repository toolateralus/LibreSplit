using System;

namespace LibreSplit;

public class ComparisonViewModel : ViewModelBase {
  public bool Signed { get; set; } = false;
  public TimeSpan? Time {
    set {
      if (Signed && value is TimeSpan ts && ts.TotalMilliseconds > 0) {
        TimeString = "+" + value.ToFormattedString();
      }
      else {
        TimeString = value.ToFormattedString();
      }
    }
  }
  private string? classes;
  public string? Classes {
    get => classes;
    set {
      classes = value;
      OnPropertyChanged();
    }
  }
  private string? timeString;
  public string? TimeString {
    get => timeString;
    set {
      timeString = value;
      OnPropertyChanged();
    }
  }
}