using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using SharpHook;
using SharpHook.Data;
using LibreSplit.IO;
namespace LibreSplit.UI;

public static partial class Input {
  private static EventLoopGlobalHook _globalEventHook = new();

  private readonly static ConcurrentDictionary<KeyCode, Action> _keyPressedHandlers = [];

  public static event Action<KeyCode>? AnyKeyPressed;

  public static KeyCode StringToKeyCode(string str) {
    var enumFormat = $"Vc{str}";
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
        Logs.LogError($"An exception occured when handling a KeyPressed event: {ex}");
      }
    }

    try {
      AnyKeyPressed?.Invoke(e.Data.KeyCode);
    }
    catch (Exception ex) {
      Logs.LogError($"An exception occured when calling the AnyKeyPressed event: {ex}");
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
      Logs.LogError($"An exception occured when stopping the global event hook input listener: {ex}");
    }
  }

  public static string KeyCodeDisplayString(KeyCode kc) => kc.ToString()[2..];

}
