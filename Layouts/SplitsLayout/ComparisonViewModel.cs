using System;

namespace LibreSplit.Layouts.SplitsLayout;

public class ComparisonViewModel : ViewModelBase {
  public bool Signed { get; set; } = false;
  public TimeSpan? Time {
    set {
      TimeString = value.ToFormattedString(Signed);
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
