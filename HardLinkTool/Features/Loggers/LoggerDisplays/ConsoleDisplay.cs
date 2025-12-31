using HardLinkTool.Features.Enums;
using HardLinkTool.Features.Interfaces;
using HardLinkTool.Features.Loggers.LoggerProcessors;
using HardLinkTool.Features.Loggers.LoggerProcessors.Modules;
using HardLinkTool.Features.Utils;

namespace HardLinkTool.Features.Loggers.LoggerDisplays;

public sealed class ConsoleDisplay : ILoggerDisplay
{
    private readonly LoggerLevel _level;

    public ConsoleDisplay(LoggerLevel level)
    {
        _level = level;
    }

    public void Log(object message)
    {
        string formattedMessage = $"[{LoggerUtils.GetLoggerLevelDisplay(_level)}: {DateTime.Now:HH:mm:ss}][{Environment.CurrentManagedThreadId}]{message}";
        ConsoleLoggerProcessor.CreateOrGetInstance()
            .AddLog(new ConsoleLoggerEntry(LoggerUtils.GetColor(_level), formattedMessage));
    }
}