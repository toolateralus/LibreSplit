using System;
using System.Collections.Generic;
using Avalonia.Input;
using LibreSplit.Timing;

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
  private bool isEditingSplits = false;
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
    if (Run == null || isEditingSplits) {
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

  internal void StartEditingSplits() {
    foreach (var (_, key) in keymap) {
      Input.UnGrabKey(key);
    }
    isEditingSplits = true;
  }
  
  internal void StopEditingSplits() {
    foreach (var (_, key) in keymap) {
      Input.GrabKey(key);
    }
    isEditingSplits = false;
  }
}