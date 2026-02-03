using Avalonia.Controls;
using Avalonia.Interactivity;
using HardLinkTool.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HardLinkTool.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenProgressDetail(object? sender, RoutedEventArgs e)
    {
        App.Current!.ServiceProvider.GetRequiredService<IWindowsManagerService>()
            .OpenWindow(WindowConstant.DETAILS_WINDOW, this);
    }

    private void OpenLicenses(object? sender, RoutedEventArgs e)
    {
        App.Current!.ServiceProvider.GetRequiredService<IWindowsManagerService>()
            .OpenWindow(WindowConstant.LICENSES_WINDOW, this);
    }

    private void OpenSettings(object? sender, RoutedEventArgs e)
    {
        App.Current!.ServiceProvider.GetRequiredService<IWindowsManagerService>()
            .OpenWindow(WindowConstant.SETTINGS_WINDOW, this);
    }
}
