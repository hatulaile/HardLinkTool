namespace HardLinkTool;

public class CreateHardLinkOption
{
    public required string Input { get; init; }

    public string? Output { get; init; }

    public long SkipSize { get; init; }

    public bool IsOverwrite { get; init; }
}