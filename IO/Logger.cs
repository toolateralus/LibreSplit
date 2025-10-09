
using System;
using System.IO;
using System.Runtime.CompilerServices;
namespace LibreSplit.IO;

public static class Logs {
  public const string LOGGER_FILE_PATH = "libresplit/logs.log";
  private static readonly StreamWriter s_logFileWriter = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), LOGGER_FILE_PATH), append: true);
  private static string FormatSourceLocation(string path, int lineNumber) {
    var fileName = Path.GetFileName(path);
    return $"{fileName}:{lineNumber}";
  }

  public static void LogInfo(object? value, [CallerMemberName] string location = "", [CallerFilePath] string path = "", [CallerLineNumber] int lineNumber = 0) {
    string sourceLocation = FormatSourceLocation(path, lineNumber);
    string message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [INFO] [{location}] [{sourceLocation}] {value}";
    s_logFileWriter.WriteLine(message);
    s_logFileWriter.Flush();
    Console.WriteLine(message);
  }

  public static void LogWarning(object? value, [CallerMemberName] string location = "", [CallerFilePath] string path = "", [CallerLineNumber] int lineNumber = 0) {
    string sourceLocation = FormatSourceLocation(path, lineNumber);
    string message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [WARN] [{location}] [{sourceLocation}] {value}";
    s_logFileWriter.WriteLine(message);
    s_logFileWriter.Flush();
    Console.WriteLine(message);
  }

  public static void LogError(object? value, [CallerMemberName] string location = "", [CallerFilePath] string path = "", [CallerLineNumber] int lineNumber = 0) {
    string sourceLocation = FormatSourceLocation(path, lineNumber);
    string message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [ERROR] [{location}] [{sourceLocation}] {value}";
    s_logFileWriter.WriteLine(message);
    s_logFileWriter.Flush();
    Console.WriteLine(message);
  }
}