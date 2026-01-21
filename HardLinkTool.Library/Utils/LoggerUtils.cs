using HardLinkTool.Library.Enums;
using HardLinkTool.Library.Features.Loggers.LoggerProcessors;

namespace HardLinkTool.Library.Utils;

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
}