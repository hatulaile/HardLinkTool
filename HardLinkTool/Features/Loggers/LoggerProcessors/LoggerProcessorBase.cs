using System.Threading.Channels;

namespace HardLinkTool.Features.Loggers.LoggerProcessors;

public abstract class LoggerProcessorBase<TLogEntry> : IDisposable, IAsyncDisposable where TLogEntry : struct
{
    private readonly Channel<TLogEntry> _channel = Channel.CreateUnbounded<TLogEntry>();

    private Task _task;

    protected LoggerProcessorBase()
    {
        _task = ProcessAsync();
    }

    private async Task ProcessAsync()
    {
        await Task.Yield();
        await foreach (TLogEntry logEntry in _channel.Reader.ReadAllAsync())
        {
            await Log(logEntry);
        }
    }

    protected abstract Task Log(TLogEntry entry);

    public virtual void AddLog(TLogEntry entry)
    {
        _channel.Writer.TryWrite(entry);
    }

    public virtual async Task AddLogAsync(TLogEntry entry)
    {
        await _channel.Writer.WriteAsync(entry);
    }

    public virtual void Dispose()
    {
        _channel.Writer.Complete();
    }

    public virtual ValueTask DisposeAsync()
    {
        _channel.Writer.Complete();
        return ValueTask.CompletedTask;
    }
    
    public async Task FlushAsync()
    {
        await DisposeAsync();
        await _task;
    }
}