using System.Runtime.InteropServices;

namespace HardLinkTool.Modules;

[StructLayout(LayoutKind.Auto)]
public readonly struct CreateHardLinkOption
{
    public CreateHardLinkOption(string target, string output, long skipSize, bool isOverwrite)
    {
        Target = target;
        Output = output;
        SkipSize = skipSize;
        IsOverwrite = isOverwrite;
    }

    public readonly string Target;

    public readonly string Output;

    public readonly long SkipSize;

    public readonly bool IsOverwrite;
}