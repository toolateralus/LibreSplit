using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LibreSplit;
public abstract class LayoutItem : ViewModelBase {
  [JsonIgnore]
  public abstract Control? Control { get; }
  [JsonIgnore]
  public abstract Control? Editor { get; }
  public abstract string LayoutItemName { get; }
}