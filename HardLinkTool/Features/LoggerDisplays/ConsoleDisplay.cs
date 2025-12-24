using HardLinkTool.Features.Enums;
using HardLinkTool.Features.Interfaces;
using HardLinkTool.Features.Utils;

namespace HardLinkTool.Features.LoggerDisplays;

public class ConsoleDisplay : ILoggerDisplay
{
    private readonly LoggerLevel _level;

    public ConsoleDisplay(LoggerLevel level)
    {
        _level = level;
    }

    public void Log(object message)
    {
        LoggerUtils.Raw($"[{LoggerUtils.GetLoggerLevelDisplay(_level)}: {DateTime.Now:HH:mm:ss}]{message}",
            LoggerUtils.GetColor(_level));
    }
}