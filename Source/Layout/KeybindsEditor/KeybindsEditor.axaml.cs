using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SharpHook.Data;

namespace LibreSplit {
  public partial class KeybindsEditor : Window {

    public string StartOrSplit { get; set; }
    public string Pause { get; set; }
    public string SkipBack { get; set; }
    public string SkipForward { get; set; }
    public string Reset { get; set; }

    public KeybindsEditor(LibreSplitContext context) {
      InitializeComponent();
      DataContext = this;

      // remove the Vc prefix from enum variants.
      static string keyCodeDisplayString(KeyCode kc) {
        return kc.ToString()[2..];
      }

      StartOrSplit = keyCodeDisplayString(context.keymap[Keybind.StartOrSplit]);
      Pause = keyCodeDisplayString(context.keymap[Keybind.Pause]);
      SkipBack = keyCodeDisplayString(context.keymap[Keybind.SkipBack]);
      SkipForward = keyCodeDisplayString(context.keymap[Keybind.SkipForward]);
      Reset = keyCodeDisplayString(context.keymap[Keybind.Reset]);

      Closing += delegate {
        context.keymap = new() {
          {Keybind.StartOrSplit, Input.StringToKeyCode(StartOrSplit)},
          {Keybind.Pause, Input.StringToKeyCode(Pause)},
          {Keybind.SkipBack, Input.StringToKeyCode(SkipBack)},
          {Keybind.SkipForward, Input.StringToKeyCode(SkipForward)},
          {Keybind.Reset, Input.StringToKeyCode(Reset)},
        };
      };
    }

  }
}