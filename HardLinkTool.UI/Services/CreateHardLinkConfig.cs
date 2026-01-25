using CommunityToolkit.Mvvm.ComponentModel;

namespace HardLinkTool.UI.Services;

public class CreateHardLinkConfig : ObservableObject, ICreateHardLinkConfig
{
    public long SkipSize
    {
        get;
        set => SetProperty(ref field, value);
    } = 1024L;

    public bool IsOverwrite
    {
        get;
        set => SetProperty(ref field, value);
    } = false;
}