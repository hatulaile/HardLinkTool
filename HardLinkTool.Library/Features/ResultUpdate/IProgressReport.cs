using HardLinkTool.Library.Modules;

namespace HardLinkTool.Library.Features.ResultUpdate;

public interface IProgressReport
{
    int UpdateInterval { get; }
    
    void Report(CreateHardLinkResults results);

    void Complete(CreateHardLinkResults results);
}