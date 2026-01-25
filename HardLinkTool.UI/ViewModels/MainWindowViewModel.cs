using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HardLinkTool.Library.Features;
using HardLinkTool.Library.Features.Loggers;
using HardLinkTool.Library.Modules;
using HardLinkTool.UI.Services;
using HardLinkTool.UI.Utils;

namespace HardLinkTool.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public const string HAND_LINK_POSTFIX = "-link";

    public IHardLinkProgressReport HardLinkProgressReport => ServiceLocator.Current.ProgressReport;
    public IDialogService DialogService => ServiceLocator.Current.DialogService;
    public IStorageProvider StorageProvider => ServiceLocator.Current.StorageProvider;
    public ICreateHardLinkConfig CreateHardLinkConfig => ServiceLocator.Current.CreateHardLinkConfig;
    public ILogger Logger => ServiceLocator.Current.Logger;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenEditableCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteEntryCommand))]
    private HardLinkEntry _selectedEntry;

    public ObservableCollection<HardLinkEntry> HardLinkEntries { get; } = [];

    [RelayCommand(CanExecute = nameof(CanAddFile))]
    public async Task AddFileAsync()
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择添加的文件",
            AllowMultiple = true,
            FileTypeFilter = [FilePickerFileTypes.All]
        });

        if (files.Count == 0) return;
        foreach (IStorageFile file in files)
        {
            HardLinkEntry entry = CreateHardLinkUtils.GetDefaultHardLinkEntry(file.Path.LocalPath, HAND_LINK_POSTFIX);
            AddEntry(entry);
        }
    }

    [RelayCommand(CanExecute = nameof(CanAddDirectoryAsync))]
    public async Task AddDirectoryAsync()
    {
        IReadOnlyList<IStorageFolder> dirs = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "选择添加的目录",
            AllowMultiple = true
        });

        if (dirs.Count == 0) return;
        foreach (IStorageFolder file in dirs)
        {
            HardLinkEntry entry = CreateHardLinkUtils.GetDefaultHardLinkEntry(file.Path.LocalPath, HAND_LINK_POSTFIX);
            AddEntry(entry);
        }
    }

    [RelayCommand(CanExecute = nameof(CanDeleteEntry))]
    public void DeleteEntry()
    {
        int index = HardLinkEntries.IndexOf(SelectedEntry);
        HardLinkEntries.Remove(SelectedEntry);
        if (HardLinkEntries.Count == 0)
        {
            SelectedEntry = default;
            return;
        }

        SelectedEntry = HardLinkEntries.Count > index ? HardLinkEntries[index] : HardLinkEntries[^1];
    }

    [RelayCommand(CanExecute = nameof(CanClearEntry))]
    public void ClearEntry()
    {
        HardLinkEntries.Clear();
        SelectedEntry = default;
    }

    [RelayCommand(CanExecute = nameof(CanOpenEditable))]
    public async Task OpenEditableAsync()
    {
        string newOutput = await DialogService.OpenEditable(SelectedEntry);
        if (SelectedEntry.Output.Equals(newOutput)) return;
        int index = HardLinkEntries.IndexOf(SelectedEntry);
        HardLinkEntries.RemoveAt(index);
        HardLinkEntries.Insert(index, new HardLinkEntry(SelectedEntry.Target, newOutput));
        SelectedEntry = new HardLinkEntry(SelectedEntry.Target, newOutput);
    }

    [RelayCommand(CanExecute = nameof(CanFilesDrop))]
    public void FilesDrop(IEnumerable<IStorageItem>? files)
    {
        if (files is null || CreateHardLinkCommand.IsRunning) return;
        foreach (IStorageItem file in files)
        {
            HardLinkEntry entry =
                CreateHardLinkUtils.GetDefaultHardLinkEntry(file.Path.LocalPath, HAND_LINK_POSTFIX);
            AddEntry(entry);
        }
    }

    [RelayCommand(CanExecute = nameof(CanCreateHardLink))]
    public async Task CreateHardLinkAsync()
    {
        HardLinkEntry[] entries = HardLinkEntries.Where(x => Path.Exists(x.Target)).ToArray();
        if (entries.Length == 0) return;

        CreateHardLinkOption option =
            new CreateHardLinkOption(entries, CreateHardLinkConfig.SkipSize, CreateHardLinkConfig.IsOverwrite);
        var handler = new CreateHardLinkHandler(Logger, HardLinkProgressReport);
        await handler.RunAsync(option);
    }


    public bool CanAddFile() => !CreateHardLinkCommand.IsRunning;
    public bool CanAddDirectoryAsync() => !CreateHardLinkCommand.IsRunning;
    public bool CanOpenEditable() => !CreateHardLinkCommand.IsRunning && HasSelectedEntry();
    public bool CanDeleteEntry() => !CreateHardLinkCommand.IsRunning && HasSelectedEntry();
    public bool CanClearEntry() => !CreateHardLinkCommand.IsRunning && HasEntry();
    public bool CanFilesDrop() => !CreateHardLinkCommand.IsRunning;
    public bool CanCreateHardLink() => !CreateHardLinkCommand.IsRunning && HasEntry();
    public bool HasEntry() => HardLinkEntries.Count != 0;
    public bool HasSelectedEntry() => !SelectedEntry.Equals(default);

    public void AddEntry(HardLinkEntry entry)
    {
        if (HardLinkEntries.All(x => x.Target != entry.Target))
        {
            HardLinkEntries.Add(entry);
        }
    }

    public void AddEntry(params Span<HardLinkEntry> entrys)
    {
        foreach (HardLinkEntry entry in entrys)
        {
            if (HardLinkEntries.All(x => x.Target != entry.Target))
            {
                HardLinkEntries.Add(entry);
            }
        }
    }

    public MainWindowViewModel()
    {
        HardLinkEntries.CollectionChanged += (_, _) =>
        {
            ClearEntryCommand.NotifyCanExecuteChanged();
            CreateHardLinkCommand.NotifyCanExecuteChanged();
        };

        CreateHardLinkCommand.PropertyChanged += (_, args) =>
        {
            if (!args.PropertyName?.Equals(nameof(CreateHardLinkCommand.IsRunning)) is false) return;
            AddFileCommand.NotifyCanExecuteChanged();
            AddDirectoryCommand.NotifyCanExecuteChanged();
            DeleteEntryCommand.NotifyCanExecuteChanged();
            OpenEditableCommand.NotifyCanExecuteChanged();
            ClearEntryCommand.NotifyCanExecuteChanged();
            OpenEditableCommand.NotifyCanExecuteChanged();
            FilesDropCommand.NotifyCanExecuteChanged();
        };
    }
}