using HardLinkTool.Features.Enums;
using HardLinkTool.Features.Interfaces;
using HardLinkTool.Features.Utils;

namespace HardLinkTool.Features.LoggerDisplays;

public class LocalFileDisplay : ILoggerDisplay
{
    private readonly string _logFilePath;
    private readonly LoggerLevel _level;

    public LocalFileDisplay(LoggerLevel level, string path)
    {
        _logFilePath = path;
        _level = level;
    }

    public void Log(object message)
    {
        File.AppendAllText(Path.GetFullPath(_logFilePath),
            $"[{LoggerUtils.GetLoggerLevelDisplay(_level)}: {DateTime.Now:HH:mm:ss}]{message}\n");
    }
}