using HardLinkTool.Features.Enums;
using HardLinkTool.Features.Loggers.LoggerProcessors;

namespace HardLinkTool.Features.Utils;

public static class LoggerUtils
{
    public static ConsoleColor GetColor(LoggerLevel level)
    {
        return level switch
        {
            LoggerLevel.Debug => ConsoleColor.DarkGray,
            LoggerLevel.Info => ConsoleColor.Cyan,
            LoggerLevel.Warn => ConsoleColor.Yellow,
            LoggerLevel.Error => ConsoleColor.Red,
            LoggerLevel.Fatal => ConsoleColor.DarkRed,
            _ => ConsoleColor.White
        };
    }
    
    public static string GetLoggerLevelDisplay(LoggerLevel level)
    {
        return level switch
        {
            LoggerLevel.Debug => "DEBUG",
            LoggerLevel.Info => "INFO",
            LoggerLevel.Warn => "WARN",
            LoggerLevel.Error => "ERROR",
            LoggerLevel.Fatal => "FATAL",
            _ => "Unknown"
        };
    }
    
    internal static async Task FlushAllLoggerProcessorAsync()
    {
        if (ConsoleLoggerProcessor.Instance is not null)
            await ConsoleLoggerProcessor.Instance.FlushAsync();

        foreach (var fileLoggerProcessor in FileLoggerProcessor.Instances)
            await fileLoggerProcessor.Value.FlushAsync();
    }
}