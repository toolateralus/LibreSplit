using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LibreSplit
{
  public partial class KeybindsEditor : Window
  {
    
    public string StartOrSplit {get; set;}
    public string Pause {get; set;}
    public string SkipBack {get; set;}
    public string SkipForward {get; set;}
    public string Reset {get; set;}
    
    public KeybindsEditor(LibreSplitContext context)
    {
      InitializeComponent();
      DataContext = this;
      
      StartOrSplit = context.keymap[Keybind.StartOrSplit];
      Pause = context.keymap[Keybind.Pause];
      SkipBack = context.keymap[Keybind.SkipBack];
      SkipForward = context.keymap[Keybind.SkipForward];
      Reset = context.keymap[Keybind.Reset];
      
      Closing += delegate {
        context.keymap = new () {
          {Keybind.StartOrSplit, StartOrSplit},
          {Keybind.Pause, Pause},
          {Keybind.SkipBack, SkipBack},
          {Keybind.SkipForward, SkipForward},
          {Keybind.Reset, Reset},
        };
      };
    }
  
  }
}