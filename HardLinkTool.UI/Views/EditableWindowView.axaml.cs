using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HardLinkTool.UI.Views;

public partial class EditableWindowView : Window
{
    public EditableWindowView()
    {
        InitializeComponent();
    }

    private void OkClick(object? sender, RoutedEventArgs e)
    {
        Close(true);
    }

    private void CancelClick(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}