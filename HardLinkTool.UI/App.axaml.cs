using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using HardLinkTool.Library.Features;
using HardLinkTool.Library.Features.Loggers;
using HardLinkTool.UI.Extensions;
using HardLinkTool.UI.ViewModels;
using HardLinkTool.UI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace HardLinkTool.UI;

public class App : Application
{
    [AllowNull] public IServiceProvider ServiceProvider { get; private set; }

    public new static App? Current => Application.Current as App;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            var services = new ServiceCollection();
            services.AddHardLinkTool();
            ServiceProvider = services.BuildServiceProvider();
            desktop.MainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            desktop.MainWindow.DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow.Closed += CleanupResourcesAsync;
            desktop.MainWindow.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }


    private async void CleanupResourcesAsync(object? sender, EventArgs args)
    {
        try
        {
            await StaticResources.FlushAsync();
        }
        catch (Exception e)
        {
            ServiceProvider.GetRequiredService<ILogger>().Fatal(e);
        }
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
