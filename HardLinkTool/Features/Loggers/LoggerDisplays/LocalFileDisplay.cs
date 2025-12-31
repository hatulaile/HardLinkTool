using HardLinkTool.Features.Enums;
using HardLinkTool.Features.Interfaces;
using HardLinkTool.Features.Loggers.LoggerProcessors;
using HardLinkTool.Features.Loggers.LoggerProcessors.Modules;
using HardLinkTool.Features.Utils;

namespace HardLinkTool.Features.Loggers.LoggerDisplays;

public sealed class LocalFileDisplay : ILoggerDisplay
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
        string path = Path.GetFullPath(_logFilePath);
        FileLoggerProcessor.CreateOrGetInstance(path).AddLog(
            new FileLoggerEntry($"[{LoggerUtils.GetLoggerLevelDisplay(_level)}: {DateTime.Now:HH:mm:ss}][{Environment.CurrentManagedThreadId}]{message}\n"));
    }
}