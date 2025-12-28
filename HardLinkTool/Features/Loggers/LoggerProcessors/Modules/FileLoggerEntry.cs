namespace HardLinkTool.Features.Loggers.LoggerProcessors.Modules;

public readonly struct FileLoggerEntry
{
    public readonly string Message;

    public FileLoggerEntry(string message)
    {
        Message = message;
    }
}