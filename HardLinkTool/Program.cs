using HardLinkTool.Commands;

namespace HardLinkTool;

public static class Program
{
    public const string HAND_LINK_POSTFIX = "-link";

    public static async Task<int> Main(string[] args)
    {
        var hardLinkCommand = new HardLinkCommand();
        return await hardLinkCommand.Parse(args).InvokeAsync();
    }
}