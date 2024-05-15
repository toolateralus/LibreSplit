#include "X11/keysym.h"
#include <X11/X.h>
#include <X11/Xlib.h>
#include <X11/XKBlib.h>
#include <cstring>

#define Success 0
#define XServerConnectionFailure 4
#define NoKeyPolled 5
#define KeySymbolStringConversionFailed 6

Display *display;
Window root;
extern "C"
int PollKey(char* key_string, size_t buffer_size) {
  if (display == NULL) {
    return XServerConnectionFailure;
  }
  XEvent xevent;
  if (XPending(display) == 0) {
    return NoKeyPolled;
  }
  XNextEvent(display, &xevent);
  if (xevent.type != KeyPress) {
    return NoKeyPolled;
  } 
  XKeyEvent key = xevent.xkey;
  auto shift_level = key.state & ShiftMask ? 1 : 0;
  auto key_symbol = XkbKeycodeToKeysym(display, key.keycode, 0, shift_level);
  const char* temp = XKeysymToString(key_symbol);
  if (temp == NULL) {
    return KeySymbolStringConversionFailed;
  }
  strncpy(key_string, temp, buffer_size - 1);
  key_string[buffer_size - 1] = '\0';  // ensure null-termination
  return Success;
}
extern "C"
int Initialize() {
  display = XOpenDisplay(NULL);
  if (display == NULL) {
    return XServerConnectionFailure;
  }
  root = RootWindow(display, DefaultScreen(display));
  return Success;
}
extern "C"
int GrabKey(char* key_string) {
  if (display == NULL) {
    return XServerConnectionFailure;
  }
  KeySym key_symbol = XStringToKeysym(key_string);
  if (key_symbol == NoSymbol) {
    return KeySymbolStringConversionFailed;
  }
  KeyCode keycode = XKeysymToKeycode(display, key_symbol);
  return XGrabKey(display, keycode, AnyModifier, root, True, GrabModeAsync, GrabModeAsync);
}
extern "C"
int UnGrabKey(char* key_string) {
  if (display == NULL) {
    return XServerConnectionFailure;
  }
  KeySym key_symbol = XStringToKeysym(key_string);
  if (key_symbol == NoSymbol) {
    return KeySymbolStringConversionFailed;
  }
  KeyCode keycode = XKeysymToKeycode(display, key_symbol);
  XUngrabKey(display, keycode, AnyModifier, root);
  return Success;
}