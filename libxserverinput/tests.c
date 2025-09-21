
#include <stddef.h>
#include <stdio.h>
#include <unistd.h>

extern int Initialize();
extern int PollKey(char *key_string, size_t buffer_size);
extern int GrabKey(const char *key_string);
extern int UnGrabKey(const char *key_string);

constexpr int buffer_size = 128;
char string[buffer_size];

int main() {
  Initialize();
  
  const char *key = "f";
  GrabKey(key);
  while (1) {
    int result = PollKey(string, buffer_size);
    if (result == 0) {
      printf("Got key %s\n", string);
    } else {
      printf("Result=%d\n", result);
    }
  }
  
  UnGrabKey(key);
}