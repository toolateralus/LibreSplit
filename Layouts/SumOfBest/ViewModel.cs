using System;
using LibreSplit.UI;
namespace LibreSplit.Layouts.SumOfBest;

public class ViewModel : ViewModelBase {
  private TimeSpan? m_sumOfBest;
  public TimeSpan? SumOfBest {
    get => m_sumOfBest;
    set {
      m_sumOfBest = value;
      OnPropertyChanged();
    }
  }
}