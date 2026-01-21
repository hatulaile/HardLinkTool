using System.Collections.Concurrent;
using HardLinkTool.Library.Features.Loggers.LoggerProcessors;

namespace HardLinkTool.Library.Features;

public static class StaticResources
{
    private static Lazy<ConsoleLoggerProcessor> _loggerProcessor = new();

    internal static ConcurrentDictionary<string, FileLoggerProcessor> FileLoggerProcessors { get; set; } = new();

    internal static ConsoleLoggerProcessor GetLoggerProcessor()
    {
        return _loggerProcessor.Value;
    }

    public static async Task FlushAsync()
    {
        if (_loggerProcessor.IsValueCreated)
        {
            await _loggerProcessor.Value.FlushAsync();
            _loggerProcessor = new Lazy<ConsoleLoggerProcessor>();
        }

        foreach (var fileLoggerProcessor in FileLoggerProcessors.Values)
            await fileLoggerProcessor.FlushAsync();
        FileLoggerProcessors.Clear();
    }
}