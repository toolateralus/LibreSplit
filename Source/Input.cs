using System;
using System.Runtime.InteropServices;
using System.Threading;
namespace LibreSplit;
public unsafe static partial class Input
{
  [LibraryImport("libxserverinput.so")]
  private static partial int PollKey(nint key_string, int buffer_size);
  [LibraryImport("libxserverinput.so")]
  private static partial int Initialize();
  [LibraryImport("libxserverinput.so")]
  private static partial int GrabKey(nint key_string);
  [LibraryImport("libxserverinput.so")]
  private static partial int UnGrabKey(nint key_string);
  private static bool running = false;
  private static bool initialized = false;
  private static readonly Thread thread = new(HandleInput);
  private static void HandleInput() {
    int buffer_size = 256;
    char* key_string = stackalloc char[buffer_size];
    while(running) {
      if (PollKey((nint)key_string, buffer_size) == 0) {
        KeyDown?.Invoke(new string(key_string));
      }
    }
  }
  public static Action<string>? KeyDown;
  public static void Start() {
    if (!initialized) {
      throw new Exception("Input not initialized");
    }
    if (running) {
      return;
    }
    running = true;
    thread.Start();
  }
  public static void Stop() {
    if (!running) {
      return;
    }
    running = false;
    thread.Join();
  }
  public static void GrabKey(string key) {
    var key_string = Marshal.StringToHGlobalAnsi(key);
    var _ = GrabKey(key_string);
    Marshal.FreeHGlobal(key_string);
  }
  public static void UnGrabKey(string key) {
    var key_string = Marshal.StringToHGlobalAnsi(key);
    var _ = UnGrabKey(key_string);
    Marshal.FreeHGlobal(key_string);
  }
  public static void InitializeInput() {
    var result = Initialize();
    if (result != 0) {
      initialized = false;
      throw new Exception("Failed to initialize input");
    } else {
      initialized = true;
    }
  }
}