namespace HardLinkTool.Library.Features.Loggers.LoggerProcessors.Modules;

public readonly struct ConsoleLoggerEntry
{
    public readonly ConsoleColor ConsoleColor;
    
    public readonly string Message;
    
    public ConsoleLoggerEntry(ConsoleColor consoleColor, string message)
    {
        ConsoleColor = consoleColor;
        Message = message;
    }
}