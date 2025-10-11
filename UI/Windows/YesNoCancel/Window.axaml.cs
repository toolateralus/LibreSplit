using System;
using System.Threading.Tasks;
using LibreSplit.IO;
namespace LibreSplit.UI.Windows.YesNoCancel;

[Flags]
public enum Result {
  /// <summary>
  /// user pressed no.
  /// </summary>
  No = 1 << 0,
  /// <summary>
  /// user pressed cancel.
  /// </summary>
  Cancel = 1 << 1,

  /// <summary>
  /// this is a distinct value comapred to No or Cancel because
  /// sometimes the consumer of the API may want a specific
  /// No or Cancel, and not just an ambiguous NoOrCancel.
  /// however, NoOrCancel is always present when No or Cancel are selected, as expected, so 
  /// for consumers who don't care which one was pressed, this is the easiest to read.
  /// </summary>
  NoOrCancel = 1 << 2,

  /// <summary>
  /// This is never present when any of the other flags are present,
  /// and same is true vice-versa.
  /// </summary>
  Yes = 1 << 3,
}

public partial class Window : Avalonia.Controls.Window {
  public Window() {
    InitializeComponent();
    var viewModel = new ViewModel();
    viewModel.SetWindow(this);
    DataContext = viewModel;
  }



  public static async Task<Result> Open(ViewModel viewModel) {
    var window = new Window() {
      DataContext = viewModel
    };

    viewModel.SetWindow(window);

    try {
      window.Show();
    }
    catch (InvalidOperationException e) {
      Logs.LogError($"An exception occured while opening a Yes/No/Cancel window: {e}");
    }
    catch (Exception e) {
      Logs.LogError($"An unknown exception occured while opening a Yes/No/Cancel window: {e}");
    }

    bool windowWasClosed = false;

    window.Closed += (_, _) => {
      windowWasClosed = true;
    };

    while (!windowWasClosed) {
      await Task.Delay(50);
    }

    return viewModel.Result;

  }
}
