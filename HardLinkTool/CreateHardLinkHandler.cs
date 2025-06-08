using static HardLinkTool.CreateHardLinkHelper;

namespace HardLinkTool;

public class CreateHardLinkHandler
{
    public string Target { get; }

    public string Output { get; }

    public long SkipSize { get; }

    public bool IsOverwrite { get; }

    private int _success;

    private int _failure;

    private int _skip;

    private int _total;

    private int _repetition;

    public CreateHardLinkHandler(string target, string output, long skipSize = 1024L,
        bool isOverwrite = false)
    {
        if (target == output)
        {
            throw new ArgumentException("目标不能与新位置相同.");
        }

        if (!IsExists(target))
        {
            throw new ArgumentException("目标不存在.");
        }

        Target = target;

        Output = output;

        SkipSize = skipSize;

        IsOverwrite = isOverwrite;
    }


    public async Task<CreateHardLinkResults> RunAsync()
    {
        if (IsFile(Target))
        {
            if (IsExists(Output))
            {
                if (!IsFile(Output))
                {
                    throw new ArgumentException("为了防止意外,不能覆盖文件夹! \n" +
                                                $"请手动删除 {Output}! ");
                }

                if (IsOverwrite)
                {
                    File.Delete(Output);
                }
                else
                {
                    throw new Exception("文件已经存在");
                }
            }

            if (SkipSize > new FileInfo(Target).Length)
            {
                File.Copy(Target, Output);
                return new CreateHardLinkResults(0, 0, 1, 0, 1);
            }

            if (CreateHardLink(Target, Output))
            {
                return new CreateHardLinkResults(1, 0, 0, 0, 1);
            }

            throw new Exception(GetLastErrorMessage());
        }

        await CreateDirectoryHardLink(Target, Output);

        return new CreateHardLinkResults(_success, _failure, _skip, _repetition, _total);
    }

    private async Task CreateDirectoryHardLink(string directory, string newDirectory)
    {
        List<Task> tasks = [];

        if (!IsExists(newDirectory))
        {
            Directory.CreateDirectory(newDirectory);
        }

        DirectoryInfo directoryInfo = new DirectoryInfo(directory);

        foreach (FileSystemInfo fileSystemInfo in directoryInfo.GetFileSystemInfos())
        {
            try
            {
                string newPath = Path.Combine(newDirectory, fileSystemInfo.Name);

                switch (fileSystemInfo)
                {
                    case FileInfo fileInfo:
                    {
                        Interlocked.Increment(ref _total);

                        if (SkipSize > 0 && SkipSize > fileInfo.Length)
                        {
                            File.Copy(fileInfo.FullName, newPath, IsOverwrite);
                            Interlocked.Increment(ref _skip);
                            continue;
                        }

                        if (File.Exists(newPath))
                        {
                            if (!IsOverwrite)
                            {
                                Interlocked.Increment(ref _repetition);
                                continue;
                            }

                            File.Delete(newPath);
                        }

                        if (CreateHardLink(fileInfo.FullName, newPath))
                        {
                            Interlocked.Increment(ref _success);
                        }
                        else
                        {
                            throw new IOException(GetLastErrorMessage());
                        }

                        break;
                    }
                    case DirectoryInfo dir:
                    {
                        tasks.Add(Task.Run(() => CreateDirectoryHardLink(dir.FullName,
                            newPath)));
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Interlocked.Increment(ref _failure);
#if DEBUG
                await Console.Error.WriteLineAsync($"创建硬链接时遇到错误: \n{e}");
#else
                await Console.Error.WriteLineAsync($"创建硬链接时遇到错误: {e.Message}");

#endif
            }
        }

        await Task.WhenAll(tasks);
    }
}