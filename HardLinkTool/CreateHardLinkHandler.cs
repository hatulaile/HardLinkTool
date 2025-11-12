using System.Diagnostics.CodeAnalysis;
using static HardLinkTool.CreateHardLinkHelper;

namespace HardLinkTool;

public class CreateHardLinkHandler
{
    public string Target { get; private set; }

    public string? Output { get; private set; }

    public long SkipSize { get; private set; }

    public bool IsOverwrite { get; private set; }

    private int _successFile;

    private int _failureFile;

    private int _skipFile;

    private int _repetitionFile;

    private int _overwriteFile;

    private int _totalFile;

    private int _newDirectory;

    private int _failureDirectory;

    private int _repetitionDirectory;

    private int _overwriteDirectory;

    private int _totalDirectory;

    public CreateHardLinkHandler(string target, string? output, long skipSize = 1024L,
        bool isOverwrite = false)
    {
        Target = target;

        Output = output;

        SkipSize = skipSize;

        IsOverwrite = isOverwrite;
    }


    [MemberNotNull(nameof(Output))]
    public async Task<CreateHardLinkResults> RunAsync()
    {
        Target = Path.GetFullPath(ProcessPathPostfix(Target));
        if (!Path.Exists(Target))
        {
            throw new ArgumentException("目标不存在.");
        }

        Output ??= GetDefaultOutput(Target, IsFile(Target), Program.HAND_LINK_POSTFIX);
        Output = Path.GetFullPath(ProcessPathPostfix(Output));

        if (Target == Output)
        {
            throw new ArgumentException("目标不能与新位置相同.");
        }

        if (IsEitherParent(Target, Output))
        {
            throw new Exception("不能将新位置设置为与目标嵌套的关系!!!! \n" +
                                "如果是未设置输出目录请截图发送 issue!!!! \n" +
                                $"Target :{Target} \n" +
                                $"Output :{Output} \n");
        }

        if (Directory.Exists(Output))
        {
            throw new ArgumentException("为了防止意外,不能覆盖文件夹! \n" +
                                        $"请手动删除 {Output}! ");
        }

        if (IsFile(Target)) await CreateFileHardLink(new FileInfo(Target), Output);
        else await CreateDirectoryHardLink(Target, Output);

        return new CreateHardLinkResults
        {
            SuccessFile = _successFile,
            FailureFile = _failureFile,
            SkipFile = _skipFile,
            RepetitionFile = _repetitionFile,
            OverwriteFile = _overwriteFile,
            TotalFile = _totalFile,
            NewDirectory = _newDirectory,
            FailureDirectory = _failureDirectory,
            RepetitionDirectory = _repetitionDirectory,
            OverwriteDirectory = _overwriteDirectory,
            TotalDirectory = _totalDirectory
        };
    }

    private async Task CreateDirectoryHardLink(string directory, string newDirectory)
    {
        await Task.Yield();
        Interlocked.Increment(ref _totalDirectory);
        if (File.Exists(newDirectory))
        {
            if (!IsOverwrite)
            {
                Interlocked.Increment(ref _failureDirectory);
                await Console.Error.WriteLineAsync($"{newDirectory} 存在文件,请删除,指定目录或者添加 -o 属性覆盖~");
                return;
            }

            Interlocked.Increment(ref _overwriteDirectory);
            File.Delete(newDirectory);
        }

        if (!Directory.Exists(newDirectory))
        {
            Directory.CreateDirectory(newDirectory);
            Interlocked.Increment(ref _newDirectory);
        }
        else
        {
            Interlocked.Increment(ref _repetitionDirectory);
        }

        var directoryInfo = new DirectoryInfo(directory);
        DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
        List<Task> tasks = new List<Task>(directoryInfos.Length);

        foreach (DirectoryInfo info in directoryInfos)
        {
            tasks.Add(CreateDirectoryHardLink(info.FullName, Path.Combine(newDirectory, info.Name)));
        }

        foreach (var info in directoryInfo.GetFiles())
        {
            await CreateFileHardLink(info, Path.Combine(newDirectory, info.Name));
        }

        await Task.WhenAll(tasks);
    }

    private async Task CreateFileHardLink(FileInfo info, string newPath)
    {
        try
        {
            Interlocked.Increment(ref _totalFile);

            if (SkipSize > 0L && SkipSize > info.Length)
            {
                File.Copy(info.FullName, newPath, IsOverwrite);
                Interlocked.Increment(ref _skipFile);
                return;
            }

            if (File.Exists(newPath))
            {
                if (!IsOverwrite)
                {
                    Interlocked.Increment(ref _repetitionFile);
                    return;
                }

                Interlocked.Increment(ref _overwriteFile);
                File.Delete(newPath);
            }

            if (CreateHardLink(info.FullName, newPath))
            {
                Interlocked.Increment(ref _successFile);
            }
            else
            {
                throw new IOException(GetLastErrorMessage());
            }
        }
        catch (Exception e)
        {
            Interlocked.Increment(ref _failureFile);
#if DEBUG
            await Console.Error.WriteLineAsync($"创建文件硬链接时遇到错误: {e}");
#else
            await Console.Error.WriteLineAsync($"创建硬链接时遇到错误: {e.Message}");
#endif
        }
    }
}