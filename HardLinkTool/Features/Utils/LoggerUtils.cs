using HardLinkTool.Features.Enums;

namespace HardLinkTool.Features.Utils;

public static class LoggerUtils
{
    private static Lock _lock = new();

    public static void Raw(string? message, ConsoleColor color)
    {
        lock (_lock)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

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