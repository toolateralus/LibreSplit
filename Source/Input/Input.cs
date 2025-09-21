using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using SharpHook;
using SharpHook.Data;
namespace LibreSplit;

public static partial class Input {
  private static CancellationTokenSource _cancellationTokenSource = new();
  private static EventLoopGlobalHook _globalEventHook = new();

  private readonly static ConcurrentDictionary<KeyCode, Action> _keyPressedHandlers = [];

  public static event Action<KeyCode>? AnyKeyPressed;

  public static KeyCode StringToKeyCode(string str) {
    var enumFormat = $"Vc{str.ToUpper()}";
    if (Enum.TryParse<KeyCode>(enumFormat, out var value)) {
      return value;
    }
    return KeyCode.VcUndefined;
  }

  private static void KeyPressed(object? _, KeyboardHookEventArgs e) {
    if (_keyPressedHandlers.TryGetValue(e.Data.KeyCode, out var handler)) {
      try {
        handler?.Invoke();
      }
      catch (Exception ex) {
        Console.WriteLine("An exception occured when handling a KeyPressed event.");
        Console.WriteLine(ex);
      }
    }

    try {
      AnyKeyPressed?.Invoke(e.Data.KeyCode);
    }
    catch (Exception ex) {
      Console.WriteLine("An exception occured when calling the AnyKeyPressed event.");
      Console.WriteLine(ex);
    }
  }

  public static void RegisterKeyPressedListener(KeyCode key, Action callback) {
    _keyPressedHandlers[key] = callback;
  }

  public static void UnregisterKeyPressedListener(KeyCode key) {
    _keyPressedHandlers.TryRemove(key, out _);
  }

  public static async Task Start() {
    _globalEventHook.KeyPressed += KeyPressed;
    await _globalEventHook.RunAsync();
  }

  public static void Pause() {
    _globalEventHook.Stop();
  }

  public static void Stop() {
    try {
      _globalEventHook.Dispose();
    }
    catch (Exception ex) {
      Console.WriteLine("An exception occured when stopping the global event hook input listener");
      Console.WriteLine(ex);
    }
  }

}