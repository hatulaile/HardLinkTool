using System;
using System.Collections.Concurrent;
using Avalonia.Controls;
using HardLinkTool.UI.ViewModels;
using HardLinkTool.UI.Views;

namespace HardLinkTool.UI.Services;

public class WindowsManagerService : IWindowsManagerService
{
    private readonly ConcurrentDictionary<string, WeakReference<Window>> _windowCache = new();

    public void OpenWindow(string window, Window? owner = null)
    {
        (Window window, ViewModelBase viewModel) view = GetWindow(window);

        if (_windowCache.TryGetValue(window, out WeakReference<Window>? reference))
        {
            if (reference.TryGetTarget(out Window? target))
            {
                target.Activate();
                return;
            }

            _windowCache.TryRemove(window, out _);
        }

        view.window.DataContext = view.viewModel;
        view.window.Closed += (sender, args) => _windowCache.TryRemove(window, out _);
        if (owner is not null) view.window.Show(owner);
        else view.window.Show();

        _windowCache.TryAdd(window, new WeakReference<Window>(view.window));
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

    private (Window window, ViewModelBase viewModel) GetWindow(string window)
    {
        return window switch
        {
            WindowConstant.DETAILS_WINDOW => (new ProgressDetailsWindows(), new ProgressDetailsWindowViewModel()),
            WindowConstant.SETTINGS_WINDOW => (new SettingsWindow(), new SettingsWindowViewModel()),
            _ => throw new ArgumentOutOfRangeException(nameof(window), window, null)
        };
    }
}

public static class WindowConstant
{
    public const string DETAILS_WINDOW = nameof(DETAILS_WINDOW);

    public const string SETTINGS_WINDOW = nameof(SETTINGS_WINDOW);
}