using Avalonia.Controls;
using Avalonia.Controls.Templates;
using HardLinkTool.UI.ViewModels;
using StaticViewLocator;

namespace HardLinkTool.UI;

[StaticViewLocator]
public partial class ViewLocator : IDataTemplate
{
    public bool Match(object? data) => data is ViewModelBase;
}
