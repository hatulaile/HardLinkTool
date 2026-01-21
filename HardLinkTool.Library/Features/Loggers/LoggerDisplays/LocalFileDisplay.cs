using System.Diagnostics.CodeAnalysis;
using HardLinkTool.Library.Enums;
using HardLinkTool.Library.Features.Loggers.LoggerProcessors;
using HardLinkTool.Library.Features.Loggers.LoggerProcessors.Modules;
using HardLinkTool.Library.Utils;

namespace HardLinkTool.Library.Features.Loggers.LoggerDisplays;

public sealed class LocalFileDisplay : ILoggerDisplay, ILoggerLevel
{
    private Lock _lock = new();
    
    private readonly string _logFilePath;
    
    public LoggerLevel Level { get; set; }

    public LocalFileDisplay(string path)
    {
        _logFilePath = path;
    }

    [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
    public void Log(object message)
    {
        string path = Path.GetFullPath(_logFilePath);
        if (StaticResources.FileLoggerProcessors.TryGetValue(path, out FileLoggerProcessor? processor))
        {
            processor.AddLog(new FileLoggerEntry(
                $"[{LoggerUtils.GetLoggerLevelDisplay(Level)}: {DateTime.Now:HH:mm:ss}][{Environment.CurrentManagedThreadId}]{message}\n"));
            return;
        }

        lock (_lock)
        {
            if (StaticResources.FileLoggerProcessors.TryGetValue(path, out processor))
            {
                processor.AddLog(new FileLoggerEntry(
                    $"[{LoggerUtils.GetLoggerLevelDisplay(Level)}: {DateTime.Now:HH:mm:ss}][{Environment.CurrentManagedThreadId}]{message}\n"));
                return;
            }
            var fileLoggerProcessor = new FileLoggerProcessor(new FileInfo(path));
            StaticResources.FileLoggerProcessors.TryAdd(path, fileLoggerProcessor);
            fileLoggerProcessor.AddLog(new FileLoggerEntry(
                $"[{LoggerUtils.GetLoggerLevelDisplay(Level)}: {DateTime.Now:HH:mm:ss}][{Environment.CurrentManagedThreadId}]{message}\n"));
        }
       
    }

}