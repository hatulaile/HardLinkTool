using System.Threading.Tasks;
using Avalonia.Controls;
using HardLinkTool.Library.Modules;
using HardLinkTool.UI.ViewModels;
using HardLinkTool.UI.Views;

namespace HardLinkTool.UI.Services;

public class DialogService : IDialogService
{
    private readonly Window _mainWindow;

    public async Task<string> OpenEditable(HardLinkEntry entry)
    {
        var window = new EditableWindow();
        var editableViewModel = new EditableWindowViewModel(entry);
        window.DataContext = editableViewModel;
        if (await window.ShowDialog<bool>(_mainWindow))
            return editableViewModel.NewOutput;

        return entry.Output;
    }

    public DialogService(Window mainWindow)
    {
        _mainWindow = mainWindow;
    }
}