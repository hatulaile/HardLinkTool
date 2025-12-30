using System.Runtime.InteropServices;

namespace HardLinkTool.Modules;

[StructLayout(LayoutKind.Auto)]
public readonly struct FileEntry
{
    public FileEntry(FileInfo target, string output)
    {
        Target = target;
        Output = output;
    }

    public readonly FileInfo Target;

    public readonly string Output;
}