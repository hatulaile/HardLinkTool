using System;
using System.IO;
using HardLinkTool.Library.Modules;

namespace HardLinkTool.UI.Utils;

public static class CreateHardLinkUtils
{
    public static HardLinkEntry GetDefaultHardLinkEntry(string input, string handLinkPostfix)
    {
        return new HardLinkEntry(Path.GetFullPath(input),
            GetDefaultOutput(input, handLinkPostfix));
    }

    private static string GetDefaultOutput(string input, string handLinkPostfix)
    {
        if (!Path.Exists(input))
            throw new Exception("Input path does not exist.");

        return Library.Utils.CreateHardLinkUtils.GetDefaultOutput(input,
            Library.Utils.CreateHardLinkUtils.IsFile(input), handLinkPostfix);
    }
}