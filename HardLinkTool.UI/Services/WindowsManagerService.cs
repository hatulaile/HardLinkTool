using System;
using System.Collections.Concurrent;
using Avalonia.Controls;
using HardLinkTool.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using ViewModelIoCGenerator;

namespace HardLinkTool.UI.Services;

public class WindowsManagerService : IWindowsManagerService
{
    private readonly ConcurrentDictionary<string, WeakReference<Window>> _windowCache = new();

    public void OpenWindow(string window, Window? owner = null)
    {
        Window view = GetWindow(window);

        if (_windowCache.TryGetValue(window, out WeakReference<Window>? reference))
        {
            if (reference.TryGetTarget(out Window? target))
            {
                target.Activate();
                return;
            }

            _windowCache.TryRemove(window, out _);
        }

        view.Closed += (sender, args) => _windowCache.TryRemove(window, out _);
        if (owner is not null) view.Show(owner);
        else view.Show();

        _windowCache.TryAdd(window, new WeakReference<Window>(view));
    }

    public void CloseWindow(string window)
    {
        if (_windowCache.TryRemove(window, out WeakReference<Window>? reference))
        {
            if (reference.TryGetTarget(out Window? target))
            {
                target.Close();
            }
        }
    }

    private Window GetWindow(string window)
    {
        return window switch
        {
            WindowConstant.DETAILS_WINDOW => App.Current!.ServiceProvider
                .GetRequiredService<ProgressDetailsWindow>(),
            WindowConstant.SETTINGS_WINDOW =>
                App.Current!.ServiceProvider.GetRequiredService<SettingsWindow>(),
            WindowConstant.LICENSES_WINDOW =>
                App.Current!.ServiceProvider.GetRequiredService<LicensesWindow>(),
            _ => throw new ArgumentOutOfRangeException(nameof(window), window, null)
        };
    }
}

public static class WindowConstant
{
    public const string DETAILS_WINDOW = nameof(DETAILS_WINDOW);

    public const string SETTINGS_WINDOW = nameof(SETTINGS_WINDOW);

    public const string LICENSES_WINDOW = nameof(LICENSES_WINDOW);
}
