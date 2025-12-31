using System.Runtime.InteropServices;

namespace HardLinkTool.Features.Loggers.LoggerProcessors.Modules;

[StructLayout(LayoutKind.Auto)]
public readonly struct FileLoggerEntry
{
    public readonly string Message;

    public FileLoggerEntry(string message)
    {
        Message = message;
    }
}