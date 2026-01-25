using Avalonia.Platform.Storage;
using HardLinkTool.Library.Features.Loggers;
using HardLinkTool.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HardLinkTool.UI;

public class ServiceLocator
{
    private readonly ServiceProvider _serviceProvider;

    public static ServiceLocator Current => App.Current!.ServiceLocator;

    public IHardLinkProgressReport ProgressReport =>
        _serviceProvider.GetRequiredService<IHardLinkProgressReport>();

    public IDialogService DialogService =>
        _serviceProvider.GetRequiredService<IDialogService>();

    public IStorageProvider StorageProvider
        => _serviceProvider.GetRequiredService<IStorageProvider>();

    public ICreateHardLinkConfig CreateHardLinkConfig =>
        _serviceProvider.GetRequiredService<ICreateHardLinkConfig>();
    
    public IWindowsManagerService WindowsManagerService =>
        _serviceProvider.GetRequiredService<IWindowsManagerService>();
    
    public ILogger Logger =>
        _serviceProvider.GetRequiredService<ILogger>();

    public ServiceLocator(ServiceCollection collection)
    {
        _serviceProvider = collection.BuildServiceProvider();
    }
}