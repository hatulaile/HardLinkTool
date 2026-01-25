using HardLinkTool.Library.Features.ResultUpdate;
using HardLinkTool.Library.Modules;

namespace HardLinkTool.UI.Services;

public interface IHardLinkProgressReport : IProgressReport
{
    CreateHardLinkResults LastResult { get; }
}