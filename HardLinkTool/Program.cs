using System.CommandLine;
using HardLinkTool.Commands;
using HardLinkTool.Features.Utils;

namespace HardLinkTool;

public static class Program
{
    public const string HAND_LINK_POSTFIX = "-link";

    public static async Task<int> Main(string[] args)
    {
        Console.CursorVisible = false;
        try
        {
            var hardLinkCommand = new HardLinkCommand();
            int code = await hardLinkCommand.Parse(args).InvokeAsync(new InvocationConfiguration()
            {
#if DEBUG
                EnableDefaultExceptionHandler = false,
#else
                EnableDefaultExceptionHandler = true,
#endif
                ProcessTerminationTimeout = TimeSpan.FromSeconds(10d)
            });
            await LoggerUtils.FlushAllLoggerProcessorAsync();
            return code;
        }
        finally
        {
            Console.CursorVisible = true;
        }
    }
}