namespace HardLinkTool.Modules;

public struct CreateHardLinkResults
{
    public required int SuccessFile { get; init; }

    public required int FailureFile { get; init; }

    public required int SkipFile { get; init; }

    public required int RepetitionFile { get; init; }

    public required int OverwriteFile { get; init; }

    public required int TotalFile { get; init; }

    public required int NewDirectory { get; init; }

    public required int FailureDirectory { get; init; }

    public required int RepetitionDirectory { get; init; }

    public required int OverwriteDirectory { get; init; }

    public required int TotalDirectory { get; init; }
    
    public required long ElapsedMilliseconds { get; init; }
}