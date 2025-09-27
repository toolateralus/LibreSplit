using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public static class TaskExtensionMethods {
  public static void FireAndForget(this Task task, bool continueWithThisThread = true, Action<Exception>? exceptionHandler = null, [CallerMemberName] string callerMemberName = "") {
    task.ContinueWith(t => {
      var handler = exceptionHandler ?? (ex => Console.WriteLine($"An exception occured in a FireAndForget method, source {callerMemberName}, exception {ex}"));
      if (t.Exception != null) {
        handler(t.Exception.InnerException ?? t.Exception);
      }
    }, TaskContinuationOptions.OnlyOnFaulted).ConfigureAwait(continueWithThisThread);
  }

}
public static class TimeSpanExtensionMethods {
  public static string ToFormattedString(this TimeSpan? timeSpan, bool signed = false) {
    if (timeSpan is not TimeSpan ts) {
      return "―";
    }

    string sign = ts.TotalMilliseconds < 0 ? "-" : signed ? "+" : "";

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
