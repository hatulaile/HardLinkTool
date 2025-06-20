using System.Runtime.InteropServices;
using System.Text;

namespace HardLinkTool;

public static partial class CreateHardLinkHelper
{
    public static bool CreateHardLink(string fileName, string newFileName) =>
        CreateHardLinkW(newFileName, fileName, IntPtr.Zero);

    public static bool IsFile(string path) =>
        Path.Exists(path) && !File.GetAttributes(path).HasFlag(FileAttributes.Directory);

    public static bool IsEitherParent(string path1, string path2) =>
        IsParent(path1, path2) || IsParent(path2, path1);

    private static bool IsParent(string path1, string path2)
    {
        DirectoryInfo info1 = new DirectoryInfo(path1.TrimEnd('/', '\\'));
        DirectoryInfo info2 = new DirectoryInfo(path2.TrimEnd('/', '\\'));
        bool isParent = false;
        while (info2.Parent != null)
        {
            if (info2.Parent.FullName.Length < info1.FullName.Length)
            {
                break;
            }

            if (info2.Parent.FullName == info1.FullName)
            {
                isParent = true;
                break;
            }

            info2 = info2.Parent;
        }

        return isParent;
    }

    public static string GetDefaultOutput(string input, bool isFile, string handLinkPostfix)
    {
        if (isFile)
        {
            string? directoryName = Path.GetDirectoryName(input);
            ArgumentNullException.ThrowIfNull(directoryName);
            return Path.Combine(directoryName,
                $"{Path.GetFileNameWithoutExtension(input)}{handLinkPostfix}{Path.GetExtension(input)}");
        }

        return $"{input.TrimEnd('/', '\\')}{handLinkPostfix}";
    }

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