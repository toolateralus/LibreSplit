
#include <stddef.h>


extern int Initialize();
extern int PollKey(char *key_string, size_t buffer_size);
extern int GrabKey(char *key_string);
extern int UnGrabKey(char *key_string);


int main() {
  Initialize();

  while (true) {

  }
}