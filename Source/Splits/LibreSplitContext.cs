using System;
using System.Collections.Generic;
using Avalonia.Input;
using LibreSplit.IO.Config;
using LibreSplit.Timing;
using Newtonsoft.Json.Linq;
using SharpHook.Data;

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

  public Dictionary<Keybind, KeyCode> keymap = new() {
    {Keybind.StartOrSplit, KeyCode.Vc1},
    {Keybind.Pause, KeyCode.Vc2},
    {Keybind.SkipBack, KeyCode.Vc3},
    {Keybind.SkipForward, KeyCode.Vc4},
    {Keybind.Reset, KeyCode.Vc5},
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

  public void UpdateActiveSegment() {
    var index = Run.SegmentIndex;

    if (index >= Run.Segments.Count || index < 0) {
      Console.WriteLine($"Index out of bounds when setting active segment.\nvalue={index}, segments count={Run.Segments.Count}.\nNOTE: There may just be no splits, then this is expected.");
      return;
    }

    ActiveSegment = Run.Segments[index];
  }

  public void ClearActiveSegment() {
    ActiveSegment = null;
  }

  internal void Initialize() {
    Timer.Initialize();
  }

  public void StartOrSplit() {
    if (Timer.Running) {
      // this returns false at the end of the run.
      if (!Run.Split(Timer)) {
        Timer.Stop();
        ClearActiveSegment();
      }
      else {
        UpdateActiveSegment();
      }
    }
    else {
      Timer.Reset();
      Run.Reset();
      Run.Start(Timer);
      UpdateActiveSegment();
    }
  }

  public void SkipBack() {
    Run.SkipBack();
    UpdateActiveSegment();
  }

  public void SkipForward() {
    Run.SkipForward();
    UpdateActiveSegment();
  }

  public void Reset() {
    Timer.Reset();
    Run.Reset();
    ClearActiveSegment();
  }

  internal void HandleInput(string key) {
    if (Run == null || isEditMode) {
      return;
    }

    Keybind bind = GetBindFromKey(key);

  }


  private Keybind GetBindFromKey(string key) {
    Keybind bind = Keybind.Invalid;





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

    UnregisterKeyMap();

    isEditMode = true;
  }

  // re engage the input
  internal void StopEditing() {
    if (!isEditMode) {
      return;
    }

    RegisterKeyMap();

    isEditMode = false;
  }

  private void RegisterKeyMap() {
    foreach (var (keybind, keycode) in keymap) {
      Action action;

      if (keybind == Keybind.Invalid) {
        continue;
      }

      action = keybind switch {
        Keybind.StartOrSplit => StartOrSplit,
        Keybind.Pause => Timer.Pause,
        Keybind.SkipBack => SkipBack,
        Keybind.SkipForward => SkipForward,
        Keybind.Reset => Reset,
        Keybind.Invalid => static () => Console.WriteLine("Invalid keybind in keymap!"),
        _ => static () => Console.WriteLine("Unknown keybind!")
      };

      Input.RegisterKeyPressedListener(keycode, action);
    }
  }

  private void UnregisterKeyMap() {
    foreach (var (_, keycode) in keymap) {
      Input.UnregisterKeyPressedListener(keycode);
    }
  }

  internal void InitializeInputAndKeymap(ConfigLoader configLoader) {

    if (configLoader.TryGetValue("keymap", out JToken? obj) && obj != null) {
      var keymap = obj.ToObject<Dictionary<Keybind, KeyCode>>() ?? throw new Exception("invalid keymap table");
      this.keymap = keymap;
    }

    Input.Start().FireAndForget();

    foreach (var (_, key) in keymap) {

    }
  }
}