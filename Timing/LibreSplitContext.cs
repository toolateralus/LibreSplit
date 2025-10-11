using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia.Controls;
using LibreSplit.IO;
using LibreSplit.Layouts;
using LibreSplit.UI;
using Newtonsoft.Json.Linq;
using SharpHook.Data;

namespace LibreSplit.Timing;

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
  private LayoutData? layoutData;
  public LayoutData LayoutData {
    get {
      layoutData ??= new();
      return layoutData;
    }
    set {
      if (layoutData is not null) {
        layoutData.Items.CollectionChanged -= LayoutDataCollectionChanged;
      }
      layoutData = value;
      layoutData.Items.CollectionChanged += LayoutDataCollectionChanged;
      UpdateLayout();
      OnPropertyChanged();
    }
  }

  void LayoutDataCollectionChanged(object? o, NotifyCollectionChangedEventArgs e) {
    UpdateLayout();
  }

  void UpdateLayout() {
    Layout.Clear();
    foreach (var layoutItemData in layoutData!.Items) {
      if (layoutItemData.Control is not null) {
        Layout.Add(layoutItemData.Control);
      }
    }
  }
  private ObservableCollection<Control> layout = [];
  public ObservableCollection<Control> Layout {
    get => layout;
    set {
      layout = value;
      OnPropertyChanged();
    }
  }
  private bool isEditMode = false;
  public Timer Timer { get; } = new();

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
      Logs.LogError($"Index out of bounds when setting active segment.\nvalue={index}, segments count={Run.Segments.Count}.\nNOTE: There may just be no splits, then this is expected.");
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
        Keybind.Invalid => () => Logs.LogWarning($"Invalid keybind '{keybind}' in keymap!"),
        _ => () => Logs.LogWarning($"Unknown keybind! '{keybind}'")
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

    RegisterKeyMap();

    Input.Start().FireAndForget();
  }
}
