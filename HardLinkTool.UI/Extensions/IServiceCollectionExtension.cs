using Avalonia.Controls;
using Avalonia.Platform.Storage;
using HardLinkTool.Library.Features.Loggers;
using HardLinkTool.Library.Features.Loggers.LoggerDisplays;
using HardLinkTool.UI.Services;
using HardLinkTool.UI.ViewModels;
using HardLinkTool.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using ViewModelIoCGenerator;

namespace HardLinkTool.UI.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddHardLinkTool(this IServiceCollection services)
    {
        services.AddSingleton<IWindowsManagerService, WindowsManagerService>();
        services.AddSingleton<IHardLinkProgressReport, ProgressReport>();
        services.AddSingleton<ICreateHardLinkConfig, CreateHardLinkConfig>();
        services.AddSingleton<IDialogService, DialogService>(sp =>
            new DialogService(sp.GetRequiredService<MainWindow>()));

        services.AddSingleton<ILogger>(new Logger()
            .AddInfoDisplay(new LocalFileDisplay("./logger.log"))
            .AddWarnDisplay(new LocalFileDisplay("./logger.log"))
            .AddErrorDisplay(new LocalFileDisplay("./logger.log"))
            .AddFatalDisplay(new LocalFileDisplay("./logger.log")));

        services.AddSingleton<IStorageProvider>(sp =>
            TopLevel.GetTopLevel(sp.GetRequiredService<MainWindow>())!.StorageProvider);

        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddViewModels();

        return services;
    }
}
