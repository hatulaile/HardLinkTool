using System.CommandLine;
using HardLinkTool.Library.Features;

namespace HardLinkTool.ConsoleApp;

public static class Program
{
    public const string HAND_LINK_POSTFIX = "-link";

    public static async Task<int> Main(string[] args)
    {
        System.Console.CursorVisible = false;
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
            await StaticResources.FlushAsync();
            return code;
        }
        finally
        {
            System.Console.CursorVisible = true;
        }
    }
}