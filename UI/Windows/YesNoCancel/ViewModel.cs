using System;

namespace LibreSplit.UI.Windows.YesNoCancel;

public class ViewModel {
  private WeakReference<Window?> m_ownerWindow = new(null);
  public string Prompt { get; set; } = "Are you sure?";
  public string YesText { get; set; } = "Yes";
  public string NoText { get; set; } = "No";
  public string CancelText { get; set; } = "Cancel";

  public Action? YesClicked;
  public Action? NoClicked;
  public Action? CancelClicked;
  public Action? AnythingButYesClicked;

  public Result Result { get; private set; } = Result.NoOrCancel;

  public ViewModel() {
    m_ownerWindow = new(null);
  }

  public void OnYes() {
    YesClicked?.Invoke();
    Result = Result.Yes; // overwrite yes on write because it cannot be both yes and no, etc.
    if (m_ownerWindow.TryGetTarget(out var win)) {
      win.Close();
    }
  }

  public void OnNo() {
    NoClicked?.Invoke();
    AnythingButYesClicked?.Invoke();
    Result |= Result.No; // write into it, since it is NoOrCancel and both No, for readers who want a specific or nonspecific answer.
    if (m_ownerWindow.TryGetTarget(out var win)) {
      win.Close();
    }
  }

  public void OnCancel() {
    CancelClicked?.Invoke();
    AnythingButYesClicked?.Invoke();
    Result |= Result.Cancel; // write into it, since it is NoOrCancel and both No, for readers who want a specific or nonspecific answer.
    if (m_ownerWindow.TryGetTarget(out var win)) {
      win.Close();
    }
  }

  public void SetWindow(Window window) {
    m_ownerWindow = new(window);
  }
}
