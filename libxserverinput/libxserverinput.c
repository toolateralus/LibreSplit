#if defined(__linux__) || defined(__gnu_linux__)

#include "X11/keysym.h"
#include <X11/X.h>
#include <X11/XKBlib.h>
#include <X11/Xlib.h>
#include <string.h>

#define Success 0
#define XServerConnectionFailure 4
#define NoKeyPolled 5
#define KeySymbolStringConversionFailed 6

Display *display;
Window root;

int PollKey(char *key_string, size_t buffer_size) {
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
  int shift_level = key.state & ShiftMask ? 1 : 0;
  KeySym key_symbol = XkbKeycodeToKeysym(display, key.keycode, 0, shift_level);
  const char *temp = XKeysymToString(key_symbol);

  if (temp == NULL) {
    return KeySymbolStringConversionFailed;
  }

  strncpy(key_string, temp, buffer_size - 1);
  key_string[buffer_size - 1] = '\0'; // ensure null-termination
  return Success;
}

int Initialize() {
  display = XOpenDisplay(NULL);
  if (display == NULL) {
    return XServerConnectionFailure;
  }
  root = RootWindow(display, DefaultScreen(display));
  return Success;
}

int GrabKey(char *key_string) {
  if (display == NULL) {
    return XServerConnectionFailure;
  }
  KeySym key_symbol = XStringToKeysym(key_string);
  if (key_symbol == NoSymbol) {
    return KeySymbolStringConversionFailed;
  }
  KeyCode keycode = XKeysymToKeycode(display, key_symbol);
  return XGrabKey(display, keycode, AnyModifier, root, True, GrabModeAsync,
                  GrabModeAsync);
}

int UnGrabKey(char *key_string) {
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

#else
#error "Cannot compile on non-linux platforms
#endif