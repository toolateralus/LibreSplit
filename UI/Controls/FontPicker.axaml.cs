using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace LibreSplit.UI.Controls;

public partial class FontPicker : UserControl {
  public static readonly StyledProperty<FontFamily?> FontProperty =
      AvaloniaProperty.Register<FontPicker, FontFamily?>(nameof(Font), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

  public FontFamily? Font {
    get => GetValue(FontProperty);
    set => SetValue(FontProperty, value);
  }

  public ObservableCollection<FontFamily> Fonts { get; } =
      new(FontManager.Current.SystemFonts.OrderBy(f => f.Name));

  public FontPicker() {
    InitializeComponent();
  }
}
