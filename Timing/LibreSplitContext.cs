using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using LibreSplit.IO;
using LibreSplit.Layouts;
using LibreSplit.UI;
using Newtonsoft.Json.Linq;
using SharpHook.Data;

namespace LibreSplit.Timing;

public enum Keybind {
  ToggleHotkeys = 0,
  StartOrSplit,
  Pause,
  SkipBack,
  SkipForward,
  Reset,
  Invalid,
}

public class LibreSplitContext : ViewModelBase {

  public string HotkeysDisabledText {
    get {
      return $"Hotkeys Disabled ({Input.KeyCodeDisplayString(keymap[Keybind.ToggleHotkeys])} to toggle)";
    }
  }

  public bool HotkeysDisabled {
    get => m_hotkeysDisabled;
    set {
      m_hotkeysDisabled = value;
      OnPropertyChanged();
    }
  }

  private bool m_hotkeysDisabled = false;

  public Dictionary<Keybind, KeyCode> keymap = new() {
    { Keybind.ToggleHotkeys, KeyCode.VcNumPad9},
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
      layoutData ??= LayoutData.Default;
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
    else if (Run.SegmentIndex == 0) {
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

  internal void Reset() {
    // get the last split time.
    var lastSplitTime = Run.Segments[^1].SplitTime;
    bool isPersonalBest = false;
    bool beatSomeBest = false;

    if (lastSplitTime != null && (lastSplitTime < Run.PersonalBest || Run.PersonalBest == null)) {
      isPersonalBest = true;
      beatSomeBest = true;
    }

    foreach (var seg in Run.Segments) {
      if (seg.SegmentTime is TimeSpan segmentTime) {
        if (segmentTime < seg.BestSegmentTime()) {
          beatSomeBest = true;
          break;
        }
      }

      if (seg.SplitTime is TimeSpan splitTime) {
        if (splitTime < seg.BestSplitTime()) {
          beatSomeBest = true;
          break;
        }
      }
    }

    if (beatSomeBest) {
      Dispatcher.UIThread.Invoke(() => UI.Windows.YesNoCancel.Window.Open(new UI.Windows.YesNoCancel.ViewModel() {
        Prompt = "Your have beaten some of your best times. Do you want to update them?",
        YesClicked = () => { Run.UpdateTimes(isPersonalBest); FinalizeReset(); },
        NoClicked = FinalizeReset,
      }));
    }
    else {
      Run.UpdateTimes(isPersonalBest);
      FinalizeReset();
    }
  }

  public void FinalizeReset() {
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

  private void RegisterKeyMap(params List<Keybind> ignored) {
    foreach (var (keybind, keycode) in keymap) {
      if (keybind == Keybind.Invalid || ignored.Contains(keybind)) {
        continue;
      }
      Action action;

      action = keybind switch {
        Keybind.ToggleHotkeys => ToggleHotkeys,
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

  private void ToggleHotkeys() {
    if (!m_hotkeysDisabled) {
      UnregisterKeyMap(Keybind.ToggleHotkeys);
      HotkeysDisabled = true;
      return;
    }
    HotkeysDisabled = false;
    RegisterKeyMap(Keybind.ToggleHotkeys);
  }

  private void UnregisterKeyMap(params List<Keybind> ignored) {
    foreach (var (bind, keycode) in keymap) {
      if (bind == Keybind.Invalid || ignored.Contains(bind)) {
        continue;
      }
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
