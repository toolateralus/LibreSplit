using Avalonia.Controls;
using Newtonsoft.Json;

namespace LibreSplit.Layouts;
public abstract class LayoutData : ViewModelBase {
  [JsonIgnore]
  public abstract Control? Control { get; }
  [JsonIgnore]
  public abstract Control? Editor { get; }
  public abstract string LayoutItemName { get; }
}
