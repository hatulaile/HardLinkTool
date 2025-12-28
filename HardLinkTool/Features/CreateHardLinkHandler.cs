using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HardLinkTool.Features.Interfaces;
using HardLinkTool.Features.Loggers;
using HardLinkTool.Modules;
using static HardLinkTool.Features.Utils.CreateHardLinkUtils;

namespace HardLinkTool.Features;

public class CreateHardLinkHandler
{
    public string Target { get; private set; }

    public string? Output { get; private set; }

    public long SkipSize { get; private set; }

    public bool IsOverwrite { get; private set; }

    private ILogger _logger;

    private IOverwriteDisplay? _overwriteDisplays;

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

    private Stopwatch? _stopwatch;

    private int _refreshTime;

    public CreateHardLinkHandler(string target, string? output, long skipSize = 1024L, bool isOverwrite = false,
        IOverwriteDisplay? overwriteDisplays = null, int refreshTime = 1000, ILogger? logger = null)
    {
        Target = target;

        Output = output;

        SkipSize = skipSize;

        IsOverwrite = isOverwrite;

        _logger = logger ?? new Logger();

        _overwriteDisplays = overwriteDisplays;

        _refreshTime = refreshTime;
    }


    [MemberNotNull(nameof(Output), nameof(_stopwatch))]
    public async Task<CreateHardLinkResults> RunAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
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

        bool isCancel = false;
        var refreshToken = CancellationTokenSource.CreateLinkedTokenSource(token);
        _stopwatch = Stopwatch.StartNew();
        try
        {
            if (IsFile(Target))
            {
                await CreateFileHardLinkAsync(new FileInfo(Target), Output, token);
            }
            else
            {
                if (_overwriteDisplays is not null)
                    _ = RefreshAsync(refreshToken.Token);
                await CreateDirectoryHardLinkAsync(Target, Output, token);
            }
        }
        catch (OperationCanceledException)
        {
            isCancel = true;
        }

        await refreshToken.CancelAsync();
        _overwriteDisplays?.Repetition();
        _stopwatch.Stop();

        return new CreateHardLinkResults
        {
            IsCancel = isCancel,
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
            TotalDirectory = _totalDirectory,
            ElapsedMilliseconds = _stopwatch.ElapsedMilliseconds,
        };
    }

    private async Task RefreshAsync(CancellationToken token)
    {
        if (_overwriteDisplays is null) return;
        await Task.Yield();
        while (true)
        {
            token.ThrowIfCancellationRequested();
            _overwriteDisplays.Overwrite($"成功 {_successFile} 个文件. " + $"失败 {_failureFile} 个文件. \n" +
                                         $"直接复制 {_skipFile} 个文件. 已存在 {_repetitionFile} 个文件. 覆盖 {_overwriteFile} 个文件. \n" +
                                         $"总共 {_totalFile} 个文件. \n\n" +
                                         $"新建 {_newDirectory} 个文件夹. " + $"无法新建 {_failureDirectory} 个文件夹. \n" +
                                         $"已存在 {_repetitionDirectory} 个文件夹. 覆盖 {_overwriteDirectory} 个文件夹. \n" +
                                         $"总共 {_totalDirectory} 个文件夹. \n\n" +
                                         $"总共耗时 {_stopwatch?.ElapsedMilliseconds ?? -1L} 毫秒. \n" +
                                         $"总共 {_totalFile + _totalDirectory} 个文件/文件夹. \n");
            await Task.Delay(_refreshTime, token);
        }
    }

    private async Task CreateDirectoryHardLinkAsync(string directory, string newDirectory, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await Task.Yield();
        Interlocked.Increment(ref _totalDirectory);
        if (File.Exists(newDirectory))
        {
            if (!IsOverwrite)
            {
                Interlocked.Increment(ref _failureDirectory);
                _logger.Error($"试图新建文件夹 {directory} 失败! \n{newDirectory} 存在文件.");
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
        Task[] tasks = new Task[directoryInfos.Length];

        int index = 0;
        foreach (DirectoryInfo info in directoryInfos)
        {
            token.ThrowIfCancellationRequested();
            tasks[index++] = CreateDirectoryHardLinkAsync(info.FullName, Path.Combine(newDirectory, info.Name), token);
        }

        foreach (var info in directoryInfo.GetFiles())
        {
            token.ThrowIfCancellationRequested();
            string newPath = Path.Combine(newDirectory, info.Name);
            try
            {
                await CreateFileHardLinkAsync(info, newPath, token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                Interlocked.Increment(ref _failureFile);
                _logger.Error($"{info.FullName} 试图创建硬链接失败, 位置: {newPath}. \n错误信息: {e}");
            }
        }

        if (Environment.CurrentManagedThreadId == 1)
            Console.WriteLine();
        await Task.WhenAll(tasks).WaitAsync(token);
    }

    private Task CreateFileHardLinkAsync(FileInfo info, string newPath, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        Interlocked.Increment(ref _totalFile);

        if (SkipSize > 0L && SkipSize > info.Length)
        {
            File.Copy(info.FullName, newPath, IsOverwrite);
            Interlocked.Increment(ref _skipFile);
            return Task.CompletedTask;
        }

        if (File.Exists(newPath))
        {
            if (!IsOverwrite)
            {
                Interlocked.Increment(ref _repetitionFile);
                return Task.CompletedTask;
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

        return Task.CompletedTask;
    }
}