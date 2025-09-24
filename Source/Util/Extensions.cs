using System;

namespace LibreSplit;

public static class TimeSpanExtensions {
  public static string ToFormattedString(this TimeSpan? timeSpan) {
    if (timeSpan is not TimeSpan ts) {
      return "―";
    }
    
    string sign = ts.TotalMilliseconds < 0 ? "-" : "";

    double milliseconds = Math.Floor(Math.Abs(ts.TotalMilliseconds));
    TimeSpan flooredTimeSpan = TimeSpan.FromMilliseconds(milliseconds);

    if (flooredTimeSpan.TotalMinutes < 1) {
      return sign + flooredTimeSpan.ToString(@"s\.ff");
    }
    else if (flooredTimeSpan.TotalHours < 1) {
      return sign + flooredTimeSpan.ToString(@"m\:ss\.ff");
    }
    else if (flooredTimeSpan.TotalDays < 1) {
      return sign + flooredTimeSpan.ToString(@"h\:mm\:ss\.ff");
    }
    else {
      var totalHours = flooredTimeSpan.Hours + (flooredTimeSpan.Days * 24);
      string minutesSeconds = flooredTimeSpan.ToString(@"mm\:ss\.ff");
      return sign + totalHours.ToString() + ":" + minutesSeconds;
    }
  }
}