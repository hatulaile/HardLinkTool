using HardLinkTool.UI.Services;

namespace HardLinkTool.UI.ViewModels;

public class ProgressDetailsWindowViewModel : ViewModelBase
{
    private readonly IHardLinkProgressReport _progressReport;
    public IHardLinkProgressReport ProgressReport => _progressReport;

    public ProgressDetailsWindowViewModel(IHardLinkProgressReport progressReport)
    {
        _progressReport = progressReport;
    }
}
