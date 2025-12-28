using HardLinkTool.Features.Loggers.LoggerProcessors.Modules;

namespace HardLinkTool.Features.Loggers.LoggerProcessors;

public class ConsoleLoggerProcessor : LoggerProcessorBase<ConsoleLoggerEntry>
{
    internal static ConsoleLoggerProcessor? Instance;

    protected ConsoleLoggerProcessor()
    {
    }

    public static ConsoleLoggerProcessor CreateOrGetInstance()
    {
        Instance ??= new ConsoleLoggerProcessor();
        return Instance;
    }

    protected override async Task Log(ConsoleLoggerEntry entry)
    {
        Console.ForegroundColor = entry.ConsoleColor;
        await Console.Out.WriteLineAsync(entry.Message);
        Console.ResetColor();
    }
}