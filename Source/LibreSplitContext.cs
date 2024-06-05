using System;
using System.Collections.Generic;
using Avalonia.Input;
using LibreSplit.IO.Config;
using LibreSplit.Timing;
using Newtonsoft.Json.Linq;

namespace LibreSplit;

public enum Keybind {
  StartOrSplit,
  Pause,
  SkipBack,
  SkipForward,
  Reset,
  Invalid,
}

public class LibreSplitContext : ViewModelBase {
  
  public Dictionary<Keybind, string> keymap = new() {
    {Keybind.StartOrSplit, "1"},
    {Keybind.Pause, "2"},
    {Keybind.SkipBack, "3"},
    {Keybind.SkipForward, "4"},
    {Keybind.Reset, "5"},
  };
  
  private RunData run = new();
  private SegmentData? activeSegment;
  private Layout layout = Layout.Default;
  private bool isEditMode = false;
  public Timer Timer { get; } = new();
  public Layout Layout {
    get => layout;
    set {
      layout = value;
      OnPropertyChanged();
    }
  }
  public RunData Run {
    get => run;
    set {
      run = value;
      OnPropertyChanged();
    }
  }
  public SegmentData? ActiveSegment {
    get => activeSegment;
    set {
      activeSegment = value;
      OnPropertyChanged();
    }
  }

  public void SetActiveSegment(SegmentData segment) {
    ActiveSegment = segment;
  }
  public void ClearActiveSegment() {
    ActiveSegment = null;
  }

  internal void Initialize() {
    Timer.Initialize();
  }

  internal void HandleInput(string key) {
    if (Run == null || isEditMode) {
      return;
    }

    Keybind bind = GetBindFromKey(key);

    switch (bind) {
      case Keybind.StartOrSplit: {
          if (Timer.Running) {
            // this returns false at the end of the run.
            if (!Run.Split(Timer)) {
              Timer.Stop();
              ClearActiveSegment();
            }
            else {
              SetActiveSegment(Run.Segments[Run.SegmentIndex]);
            }
          }
          else {
            Timer.Reset();
            Run.Reset();
            Run.Start(Timer);
            SetActiveSegment(Run.Segments[Run.SegmentIndex]);
          }

        }
        break;
      case Keybind.Pause: {
          Timer.Pause();
        }
        break;
      case Keybind.SkipBack: {
          Run.SkipBack();
          SetActiveSegment(Run.Segments[Run.SegmentIndex]);
        }
        break;
      case Keybind.SkipForward: {
          Run.SkipForward();
          SetActiveSegment(Run.Segments[Run.SegmentIndex]);
        }
        break;
      case Keybind.Reset: {
          Timer.Reset();
          Run.Reset();
          ClearActiveSegment();
        }
        break;
    }
  }

  private Keybind GetBindFromKey(string key) {
    Keybind bind = Keybind.Invalid;
    foreach (var (binding, key_str) in keymap) {
      if (key_str == key) {
        bind = binding;
      }
    }

    if (bind == Keybind.Invalid) {
      throw new InvalidOperationException($"Cannot use {key} :: not a valid keybind.");
    }
    
    return bind;
  }
  
  // Disengage the input while we're editing
  internal void StartEditing() {
    if (isEditMode) {
      return;
    }
    
    foreach (var (_, key) in keymap) {
      Input.UnGrabKey(key);
    }
    isEditMode = true;
  }
  
  // re engage the input
  internal void StopEditing() {
    if (!isEditMode) {
      return;
    }
    
    foreach (var (_, key) in keymap) {
      Input.GrabKey(key);
    }
    isEditMode = false;
  }

  internal void InitializeInputAndKeymap(ConfigLoader configLoader) {
    if (configLoader.TryGetValue("keymap", out JToken? obj) && obj != null) {
     var keymap = obj.ToObject<Dictionary<Keybind, string>>() ?? throw new Exception("invalid keymap table");
     this.keymap = keymap;
    }
    Input.InitializeInput();
    foreach (var (_, key_string) in keymap) {
      Input.GrabKey(key_string);
    }
    Input.Start();
  }
}