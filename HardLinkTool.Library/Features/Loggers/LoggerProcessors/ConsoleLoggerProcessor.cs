using HardLinkTool.Library.Features.Loggers.LoggerProcessors.Modules;

namespace HardLinkTool.Library.Features.Loggers.LoggerProcessors;

public sealed class ConsoleLoggerProcessor : LoggerProcessorBase<ConsoleLoggerEntry>
{
    public ConsoleLoggerProcessor()
    {
    }

    protected override async Task Log(ConsoleLoggerEntry entry)
    {
        System.Console.ForegroundColor = entry.ConsoleColor;
        await System.Console.Out.WriteLineAsync(entry.Message);
        System.Console.ResetColor();
    }
}