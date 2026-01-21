using System.Runtime.InteropServices;

namespace HardLinkTool.Library.Modules;

[StructLayout(LayoutKind.Auto)]
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