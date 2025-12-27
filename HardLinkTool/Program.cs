using HardLinkTool.Commands;

namespace HardLinkTool;

public static class Program
{
    public const string HAND_LINK_POSTFIX = "-link";

    public static async Task<int> Main(string[] args)
    {
        Console.CursorVisible = false;
        var hardLinkCommand = new HardLinkCommand();
        int code = await hardLinkCommand.Parse(args).InvokeAsync();
        Console.CursorVisible = true;
        return code;
    }
}