using System.Text;
using HardLinkTool.Features.Enums;
using HardLinkTool.Features.Loggers.LoggerDisplays;
using HardLinkTool.Features.Loggers.LoggerProcessors.Modules;

namespace HardLinkTool.Features.Loggers.LoggerProcessors;

public sealed class FileLoggerProcessor : LoggerProcessorBase<FileLoggerEntry>
{
    internal static readonly Dictionary<string, FileLoggerProcessor> Instances = new();
    private static Logger? _logger;

    private FileStream _fileStream;

    private FileLoggerProcessor(FileStream stream)
    {
        _fileStream = stream;
        _logger = new Logger().AddDebugDisplay(new ConsoleDisplay(LoggerLevel.Debug))
            .AddInfoDisplay(new ConsoleDisplay(LoggerLevel.Info))
            .AddWarnDisplay(new ConsoleDisplay(LoggerLevel.Warn))
            .AddErrorDisplay(new ConsoleDisplay(LoggerLevel.Error))
            .AddFatalDisplay(new ConsoleDisplay(LoggerLevel.Fatal));
    }

    public static FileLoggerProcessor CreateOrGetInstance(string filePath)
    {
        if (Instances.TryGetValue(filePath, out FileLoggerProcessor? value)) return value;
        var processor = new FileLoggerProcessor(new FileStream(filePath, FileMode.Append));
        Instances[filePath] = processor;
        return Instances[filePath];
    }

    protected override async Task Log(FileLoggerEntry entry)
    {
        try
        {
            await _fileStream.WriteAsync(Encoding.UTF8.GetBytes(entry.Message));
        }
        catch (Exception e)
        {
            _logger?.Error(e.Message);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _fileStream.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _fileStream.DisposeAsync();
    }
}