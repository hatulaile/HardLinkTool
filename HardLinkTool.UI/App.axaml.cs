using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using HardLinkTool.Library.Features;
using HardLinkTool.Library.Features.Loggers;
using HardLinkTool.Library.Features.Loggers.LoggerDisplays;
using HardLinkTool.UI.Services;
using HardLinkTool.UI.ViewModels;
using HardLinkTool.UI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace HardLinkTool.UI;

public partial class App : Application
{
    [AllowNull] public ServiceLocator ServiceLocator { get; private set; }

    public new static App? Current => Application.Current as App;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IWindowsManagerService, WindowsManagerService>();
            serviceCollection.AddSingleton<IHardLinkProgressReport, ProgressReport>();
            serviceCollection.AddSingleton<IWindowsManagerService, WindowsManagerService>();
            serviceCollection.AddSingleton<ICreateHardLinkConfig, CreateHardLinkConfig>();
            serviceCollection.AddSingleton<IDialogService, DialogService>(_ => new DialogService(desktop.MainWindow));
            serviceCollection.AddSingleton<IStorageProvider>(_ =>
                TopLevel.GetTopLevel(desktop.MainWindow)!.StorageProvider);

            ILogger logger = new Logger()
                .AddInfoDisplay(new LocalFileDisplay("./logger.log"))
                .AddWarnDisplay(new LocalFileDisplay("./logger.log"))
                .AddErrorDisplay(new LocalFileDisplay("./logger.log"))
                .AddFatalDisplay(new LocalFileDisplay("./logger.log"));
            serviceCollection.AddSingleton(logger);
            ServiceLocator = new ServiceLocator(serviceCollection);
            desktop.MainWindow.DataContext = new MainWindowViewModel();
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
            ServiceLocator.Logger.Fatal(e);
        }
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        // var dataValidationPluginsToRemove =
        //     BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();
        //
        // // remove each entry found
        // foreach (var plugin in dataValidationPluginsToRemove)
        // {
        //     BindingPlugins.DataValidators.Remove(plugin);
        // }
    }
}