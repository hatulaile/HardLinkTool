using System.Text;
using HardLinkTool.Library.Enums;
using HardLinkTool.Library.Features.Loggers.LoggerDisplays;
using HardLinkTool.Library.Features.Loggers.LoggerProcessors.Modules;

namespace HardLinkTool.Library.Features.Loggers.LoggerProcessors;

public sealed class FileLoggerProcessor : LoggerProcessorBase<FileLoggerEntry>
{
    private static Logger? _logger;

    private readonly FileStream _fileStream;

    public FileLoggerProcessor(FileInfo info) : this(info.OpenWrite())
    {
        if (!info.Exists)
            info.Create();
    }

    public FileLoggerProcessor(FileStream stream)
    {
        _fileStream = stream;
        _logger = new Logger().AddDebugDisplay(new ConsoleDisplay())
            .AddInfoDisplay(new ConsoleDisplay())
            .AddWarnDisplay(new ConsoleDisplay())
            .AddErrorDisplay(new ConsoleDisplay())
            .AddFatalDisplay(new ConsoleDisplay());
    }

    protected override async Task Log(FileLoggerEntry entry)
    {
        try
        {
            await _fileStream.WriteAsync(Encoding.UTF8.GetBytes(entry.Message)).ConfigureAwait(false);
            await _fileStream.FlushAsync();
        }
        catch (Exception e)
        {
            _logger?.Error(e.Message);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _fileStream.Flush();
        _fileStream.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _fileStream.FlushAsync();
        await _fileStream.DisposeAsync();
    }
}