using HardLinkTool.Library.Enums;
using HardLinkTool.Library.Features.Loggers.LoggerProcessors;
using HardLinkTool.Library.Features.Loggers.LoggerProcessors.Modules;
using HardLinkTool.Library.Utils;

namespace HardLinkTool.Library.Features.Loggers.LoggerDisplays;

public sealed class ConsoleDisplay : ILoggerDisplay, ILoggerLevel
{
    public LoggerLevel Level { get; set; }
    
    public ConsoleDisplay()
    {
    }

    public void Log(object message)
    {
        string formattedMessage =
            $"[{LoggerUtils.GetLoggerLevelDisplay(Level)}: {DateTime.Now:HH:mm:ss}][{Environment.CurrentManagedThreadId}]{message}";
        StaticResources.GetLoggerProcessor().AddLog(new ConsoleLoggerEntry(LoggerUtils.GetColor(Level), formattedMessage));
    }

}