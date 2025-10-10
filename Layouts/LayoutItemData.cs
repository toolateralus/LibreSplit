using Avalonia.Controls;
using LibreSplit.UI;
using Newtonsoft.Json;

namespace LibreSplit.Layouts;

public abstract class LayoutItemData: ViewModelBase {
  [JsonIgnore]
  public abstract Control? Control { get; }
  [JsonIgnore]
  public abstract Control? Editor { get; }
  public abstract string LayoutItemName { get; }
}
