using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using HardLinkTool.Library.Modules;
using HardLinkTool.UI.ViewModels;
using HardLinkTool.UI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace HardLinkTool.UI.Services;

public class DialogService : IDialogService
{
    private readonly Window _mainWindow;

    public async Task<string> OpenEditable(HardLinkEntry entry)
    {
        var window = App.Current!.ServiceProvider.GetRequiredService<EditableWindowView>();
        EditableWindowViewModel viewModel = (EditableWindowViewModel)window.DataContext!;
        viewModel.HardLinkEntry = entry;
        if (window is null) throw new NullReferenceException("Failed to create window.");
        window.DataContext = window;
        if (await window.ShowDialog<bool>(_mainWindow))
            return viewModel.NewOutput;

        return entry.Output;
    }

    public DialogService(Window mainWindow)
    {
        _mainWindow = mainWindow;
    }
}
