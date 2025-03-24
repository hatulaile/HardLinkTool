using System.Runtime.InteropServices;
using System.Text;

namespace HardLinkTool;

public static partial class CreateHardLinkHelper
{
    public static bool CreateHardLink(string fileName, string newFileName) =>
        CreateHardLinkW(newFileName, fileName, IntPtr.Zero);

    public static bool IsFile(string path) =>
        !File.GetAttributes(path).HasFlag(FileAttributes.Directory);

    public static bool IsExists(string path) =>
        File.Exists(path) || Directory.Exists(path);

    [LibraryImport("Kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CreateHardLinkW(
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

    [LibraryImport("kernel32.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvStdcall)])]
    private static partial uint GetLastError();

    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern int FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId,
        [Out] StringBuilder lpBuffer, uint nSize, IntPtr arguments);
}