using System.Runtime.InteropServices;

namespace HardLinkTool.Library.Modules;

[StructLayout(LayoutKind.Auto)]
public readonly struct CreateHardLinkOption
{
    public CreateHardLinkOption(HardLinkEntry[] hardLinkEntry, long skipSize, bool isOverwrite)
    {
        HardLinkEntry = hardLinkEntry;
        SkipSize = skipSize;
        IsOverwrite = isOverwrite;
    }

    public readonly HardLinkEntry[] HardLinkEntry;

    public readonly long SkipSize;

    public readonly bool IsOverwrite;
}