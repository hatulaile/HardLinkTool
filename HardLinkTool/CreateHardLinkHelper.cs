using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualBasic;

namespace HardLinkTool;

public static class CreateHardLinkHelper
{
    public static bool CreateHardLink(string fileName, string newFileName) =>
        CreateHardLinkW(newFileName, fileName, IntPtr.Zero);

    public static bool IsFile(string path) =>
        !File.GetAttributes(path).HasFlag(FileAttributes.Directory);

    public static bool IsExists(string path)
    {
        if (File.Exists(path))
        {
            return true;
        }

        return Directory.Exists(path);
    }

    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern bool CreateHardLinkW(
        string lpFileName,
        string lpExistingFileName,
        IntPtr lpSecurityAttributes
    );

    public static string GetLastErrorMessage()
    {
        StringBuilder lpBuffer = new StringBuilder(260);
        FormatMessage(0x1000 | 0x200, IntPtr.Zero, GetLastError(), 0, lpBuffer, 260, IntPtr.Zero);
        return lpBuffer.ToString();
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern uint GetLastError();

    [DllImport("Kernel32.dll")]
    private static extern int FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId,
        [Out] StringBuilder lpBuffer, uint nSize, IntPtr arguments);
}