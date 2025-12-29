namespace HardLinkTool.Modules;

public readonly struct DirectoryEntry
{
    public DirectoryEntry(DirectoryInfo target, string output)
    {
        Target = target;
        Output = output;
    }
    
    public readonly DirectoryInfo Target;
    
    public readonly string Output;
}