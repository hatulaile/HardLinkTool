using HardLinkTool.UI.Services;

namespace HardLinkTool.UI.ViewModels;

public class ProgressDetailsWindowViewModel : ViewModelBase
{
    public IHardLinkProgressReport ProgressReport => ServiceLocator.Current.ProgressReport;
    
    public ProgressDetailsWindowViewModel()
    {
    }
}