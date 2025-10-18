using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using SharpHook;
using SharpHook.Data;
using LibreSplit.IO;
using System.Collections.Generic;
namespace LibreSplit.UI;

public static partial class Input {
  private static readonly EventLoopGlobalHook _globalEventHook = new();
  private static readonly Dictionary<KeyCode, bool> _keyPressedStates = [];
  private static readonly ConcurrentDictionary<KeyCode, Action> _keyPressedHandlers = [];

  public static event Action<KeyCode>? AnyKeyPressed;

  public static KeyCode StringToKeyCode(string str) {
    var enumFormat = $"Vc{str}";
    if (Enum.TryParse<KeyCode>(enumFormat, out var value)) {
      return value;
    }
    return KeyCode.VcUndefined;
  }
  private static void KeyReleased(object? _, KeyboardHookEventArgs e) {
    _keyPressedStates.Remove(e.Data.KeyCode);
  }

  private static void KeyPressed(object? _, KeyboardHookEventArgs e) {
    if (_keyPressedHandlers.TryGetValue(e.Data.KeyCode, out var handler)) {
      // Ignore double presses / repeats, TODO: implement some kind of timer here for 
      // 'double press' protection or whatever livesplit has, and put it under some option 
      if (_keyPressedStates.ContainsKey(e.Data.KeyCode)) {
        return;
      }
      try {
        handler?.Invoke();
      }
      catch (Exception ex) {
        Logs.LogError($"An exception occured when handling a KeyPressed event: {ex}");
      }
      _keyPressedStates[e.Data.KeyCode] = true;
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
    _globalEventHook.KeyReleased += KeyReleased;
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
