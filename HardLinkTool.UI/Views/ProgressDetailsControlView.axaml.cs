using Avalonia;
using Avalonia.Controls.Primitives;

namespace HardLinkTool.UI.Views;

public class ProgressDetailsControlView : TemplatedControl
{
    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<ProgressDetailsControlView, string>(
        nameof(Title));

    public static readonly StyledProperty<string> ValueProperty = AvaloniaProperty.Register<ProgressDetailsControlView, string>(
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