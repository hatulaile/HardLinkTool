using System.Runtime.InteropServices;

namespace HardLinkTool.Modules;

[StructLayout(LayoutKind.Auto)]
public readonly struct HardLinkEntry
{
    public readonly string Target;
    
    public readonly string Output;
    
    public HardLinkEntry(string target, string output)
    {
        Target = target;
        Output = output;
    }
}