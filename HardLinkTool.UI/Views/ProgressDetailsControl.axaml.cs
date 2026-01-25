using Avalonia;
using Avalonia.Controls.Primitives;

namespace HardLinkTool.UI.Views;

public class ProgressDetailsControl : TemplatedControl
{
    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<ProgressDetailsControl, string>(
        nameof(Title));

    public static readonly StyledProperty<string> ValueProperty = AvaloniaProperty.Register<ProgressDetailsControl, string>(
        nameof(Value));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public string Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}