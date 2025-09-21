


using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SharpHook.Data;

public static class ExtensionMethods {
  public static void FireAndForget(this Task task, bool continueWithThisThread = true, Action<Exception>? exceptionHandler = null, [CallerMemberName] string callerMemberName = "") {
    task.ContinueWith(t => {
      var handler = exceptionHandler ?? (ex => Console.WriteLine($"An exception occured in a FireAndForget method, source {callerMemberName}, exception {ex}"));
      if (t.Exception != null) {
        handler(t.Exception.InnerException ?? t.Exception);
      }
    }, TaskContinuationOptions.OnlyOnFaulted).ConfigureAwait(continueWithThisThread);
  }

}