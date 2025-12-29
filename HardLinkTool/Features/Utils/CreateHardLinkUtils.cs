using System.Runtime.InteropServices;
using System.Text;

namespace HardLinkTool.Features.Utils;

public static partial class CreateHardLinkUtils
{
    private static readonly char[] Postfix = ['/', '\\', '\'', '\"'];

    public static string ProcessPathPostfix(string path)
    {
        return path.TrimEnd(Postfix);
    }

    public static bool TryCreateHardLink(string fileName, string newFileName)
    {
        return CreateHardLinkW(newFileName, fileName, IntPtr.Zero);
    }

    public static bool IsFile(string path) =>
        Path.Exists(path) && !File.GetAttributes(path).HasFlag(FileAttributes.Directory);

    public static bool IsEitherParent(string path1, string path2) =>
        IsParent(path1, path2) || IsParent(path2, path1);

    private static bool IsParent(string path1, string path2)
    {
        DirectoryInfo info1 = new DirectoryInfo(ProcessPathPostfix(path1));
        DirectoryInfo info2 = new DirectoryInfo(ProcessPathPostfix(path2));
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

        return $"{ProcessPathPostfix(input)}{handLinkPostfix}";
    }

    [LibraryImport("Kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CreateHardLinkW(
        string lpFileName,
        string lpExistingFileName,
        IntPtr lpSecurityAttributes
    );
}