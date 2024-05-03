using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using LibreSplit.Timing;

namespace LibreSplit;
public partial class SplitsControl : UserControl {
  public SplitsControl(MainWindowVM vm) {
    DataContext = vm;
    InitializeComponent();
  }
}