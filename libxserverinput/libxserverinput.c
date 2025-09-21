#if defined(__linux__) || defined(__gnu_linux__)

#include <time.h>
#include <stdio.h>
#include "X11/keysym.h"
#include <X11/X.h>
#include <X11/XKBlib.h>
#include <X11/Xlib.h>
#include <string.h>

enum {
  RESULT_SUCCESS = 0,
  RESULT_X_SERVER_CONNECTION_FAILURE = 4,
  RESULT_NO_KEY_POLLED = 5,
  RESULT_KEY_SYMBOL_STRING_CONVERSION_FAILED = 6,
};

Display *display;
Window root;

long start_time;

#define DO_LOG

#ifdef DO_LOG
#define LOG(s) printf("%ld [LIB_X_SERVER_INPUT]: %s\n", time(NULL) - start_time, s);
#else
#define LOG(s)
#endif

int PollKey(char *key_string, size_t buffer_size) {
  if (display == NULL) {
    return RESULT_X_SERVER_CONNECTION_FAILURE;
  }

  XEvent xevent;

  if (XPending(display) == 0) {
    LOG("No key at poll time");
    return RESULT_NO_KEY_POLLED;
  }

  XNextEvent(display, &xevent);
  while (xevent.type != KeyPress && (XPending(display) != 0)) {
    LOG("Non key pressed events draining");
  }

  if (XPending(display) == 0) {
    LOG("No key event after draining");
    return RESULT_NO_KEY_POLLED;
  }

  XKeyEvent key = xevent.xkey;
  int shift_level = key.state & ShiftMask ? 1 : 0;
  KeySym key_symbol = XkbKeycodeToKeysym(display, key.keycode, 0, shift_level);
  const char *temp = XKeysymToString(key_symbol);

  if (temp == NULL) {
    LOG("Result null");
    return RESULT_KEY_SYMBOL_STRING_CONVERSION_FAILED;
  }

  strncpy(key_string, temp, buffer_size - 1);
  key_string[buffer_size - 1] = '\0'; // ensure null-termination

  return RESULT_SUCCESS;
}

int Initialize() {
  start_time = time(NULL);
  display = XOpenDisplay(NULL);
  if (display == NULL) {
    return RESULT_X_SERVER_CONNECTION_FAILURE;
  }
  root = RootWindow(display, DefaultScreen(display));
  return Success;
}

int GrabKey(char *key_string) {
  if (display == NULL) {
    return RESULT_X_SERVER_CONNECTION_FAILURE;
  }
  KeySym key_symbol = XStringToKeysym(key_string);
  if (key_symbol == NoSymbol) {
    return RESULT_KEY_SYMBOL_STRING_CONVERSION_FAILED;
  }
  KeyCode keycode = XKeysymToKeycode(display, key_symbol);
  return XGrabKey(display, keycode, AnyModifier, root, True, GrabModeAsync,
                  GrabModeAsync);
}

int UnGrabKey(char *key_string) {
  if (display == NULL) {
    return RESULT_X_SERVER_CONNECTION_FAILURE;
  }

  KeySym key_symbol = XStringToKeysym(key_string);
  if (key_symbol == NoSymbol) {
    return RESULT_KEY_SYMBOL_STRING_CONVERSION_FAILED;
  }

  KeyCode keycode = XKeysymToKeycode(display, key_symbol);
  XUngrabKey(display, keycode, AnyModifier, root);
  return Success;
}

#else
#error "Cannot compile on non-linux platforms
#endif