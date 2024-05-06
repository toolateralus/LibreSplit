using System.ComponentModel;
namespace LibreSplit.ObjectModel;
public class Observable<T>(T value) : INotifyPropertyChanged, INotifyPropertyChanging {
  T innerValue = value;
  public T Value {
    get => innerValue;
    set {
      if (value != null && !value.Equals(innerValue) ||
          value == null && innerValue != null) {
        PropertyChanging?.Invoke(this, valueChangingEventArgs);
        innerValue = value;
        PropertyChanged?.Invoke(this, propertyChangedEventArgs);
      }
    }
  }
  readonly PropertyChangingEventArgs valueChangingEventArgs = new(nameof(Value));
  readonly PropertyChangedEventArgs propertyChangedEventArgs = new(nameof(Value));
  public event PropertyChangedEventHandler? PropertyChanged;
  public event PropertyChangingEventHandler? PropertyChanging;
  public static implicit operator T(Observable<T> observableProperty) => observableProperty.Value;
  public static implicit operator Observable<T>(T value) => new(value);
}
