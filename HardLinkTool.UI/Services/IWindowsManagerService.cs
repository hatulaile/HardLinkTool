using Avalonia.Controls;

namespace HardLinkTool.UI.Services;

public interface IWindowsManagerService
{
    void OpenWindow(string window, Window? owner = null);

    void CloseWindow(string window);
}