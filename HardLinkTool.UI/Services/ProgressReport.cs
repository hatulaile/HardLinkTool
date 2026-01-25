using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using HardLinkTool.Library.Modules;

namespace HardLinkTool.UI.Services;

public class ProgressReport : ObservableObject, IHardLinkProgressReport
{
    public CreateHardLinkResults LastResult
    {
        get;
        private set => SetProperty(ref field, value);
    } = new();

    public int UpdateInterval => 50;

    public void Report(CreateHardLinkResults results)
    {
        Dispatcher.UIThread.Post(() => LastResult = results);
    }

    public void Complete(CreateHardLinkResults results)
    {
        Dispatcher.UIThread.Post(() => LastResult = results);
    }
}