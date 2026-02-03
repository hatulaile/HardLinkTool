using System.IO;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HardLinkTool.Library.Modules;
using HardLinkTool.Library.Utils;
using HardLinkTool.UI.Models;

namespace HardLinkTool.UI.ViewModels;

public partial class EditableWindowViewModel : ViewModelBase
{
    private readonly IStorageProvider _storageProvider;

    public IStorageProvider StorageProvider => _storageProvider;

    public HardLinkEntry HardLinkEntry
    {
        get;
        internal set
        {
            field = value;
            NewOutput = field.Output;
        }
    }

    [ObservableProperty] private string _newOutput = string.Empty;

    partial void OnNewOutputChanged(string value)
    {
        ValidateOutput();
    }

    [ObservableProperty] private ValidateResult _validateResult;


    [RelayCommand]
    public void ValidateOutput()
    {
        if (HardLinkEntry.Output == NewOutput)
        {
            ValidateResult = new ValidateResult(false, "新路径不能与旧路径一致", Colors.PaleVioletRed);
            return;
        }

        if (HardLinkEntry.Target == NewOutput)
        {
            ValidateResult = new ValidateResult(false, "不能与源路径一致", Colors.PaleVioletRed);
            return;
        }

        if (CreateHardLinkUtils.IsEitherParent(HardLinkEntry.Target, NewOutput))
        {
            ValidateResult = new ValidateResult(false, "新路径不能是旧路径的父级目录", Colors.PaleVioletRed);
            return;
        }

        ValidateResult = new ValidateResult(true, "可用路径", Colors.ForestGreen);
    }

    [RelayCommand]
    public async Task OpenFile()
    {
        if (CreateHardLinkUtils.IsFile(HardLinkEntry.Target))
        {
            SaveFilePickerResult fileResult = await _storageProvider.SaveFilePickerWithResultAsync(
                new FilePickerSaveOptions
                {
                    Title = "选择存放的文件位置",
                    SuggestedFileName = Path.GetFileNameWithoutExtension(HardLinkEntry.Target),
                    DefaultExtension = Path.GetExtension(HardLinkEntry.Target).Remove(0, 1),
                });
            if (fileResult.File is null) return;
            NewOutput = fileResult.File.Path.LocalPath;
        }
        else
        {
            var folders =
                await _storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions { Title = "选择存放的文件夹", });

            if (folders.Count == 0)
                return;
            NewOutput = Path.Combine(folders[0].Path.LocalPath,
                Path.GetFileName(Path.TrimEndingDirectorySeparator(HardLinkEntry.Target)));
        }
    }

    public EditableWindowViewModel(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }
}
