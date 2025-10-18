using System;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using LibreSplit.UI.Windows;

namespace LibreSplit.Layouts.SumOfBest;

public partial class Control : UserControl {
  private readonly ViewModel m_viewModel;
  public Control(ViewModel viewModel) {
    InitializeComponent();
    DataContext = m_viewModel = viewModel;
  }

  private void CalculateSumOfBest() {
    TimeSpan sumOfBest = new();
    foreach (var segment in MainWindow.GlobalContext.Run.Segments) {
      if (segment.BestSegmentTime() is TimeSpan bestTime) {
        sumOfBest += bestTime;
      }
      else {
        m_viewModel.SumOfBest = null;
        return;
      }
    }
    m_viewModel.SumOfBest = sumOfBest;
  }

  protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
    MainWindow.GlobalContext.Timer.OnTick += OnTimerTick;
    base.OnAttachedToLogicalTree(e);
    CalculateSumOfBest();
  }

  private void OnTimerTick(TimeSpan span) {
    CalculateSumOfBest();
  }

  protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e) {
    MainWindow.GlobalContext.Timer.OnTick -= OnTimerTick;
    base.OnDetachedFromLogicalTree(e);
  }
}
