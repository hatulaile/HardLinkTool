using System.Diagnostics;
using System.Threading.Channels;
using HardLinkTool.Features.Interfaces;
using HardLinkTool.Features.Loggers;
using HardLinkTool.Modules;
using Microsoft.VisualBasic.FileIO;
using static HardLinkTool.Features.Utils.CreateHardLinkUtils;

namespace HardLinkTool.Features;

public sealed class CreateHardLinkHandler
{
    private Stopwatch? _stopwatch;

    private readonly CreateHardLinkOption _option;

    private readonly CreateHardLinkResults _results;

    private readonly ILogger _logger;

    private readonly IOverwriteDisplay? _overwriteDisplays;

    private readonly int _refreshTime;

    private readonly Channel<FileEntry> _fileChannel;

    private readonly Channel<DirectoryEntry> _directoryChannel;

    private Task[]? _directoryProcessor;
    private Task[]? _fileProcessor;

    private readonly EnumerationOptions _processDirectoryEntriesEnumerationOptions = new()
    {
        IgnoreInaccessible = true,
        AttributesToSkip = FileAttributes.System | FileAttributes.Hidden,
        RecurseSubdirectories = false,
        MatchType = MatchType.Simple,
        ReturnSpecialDirectories = false
    };

    private readonly EnumerationOptions _processFileEntriesEnumerationOptions = new()
    {
        IgnoreInaccessible = true,
        AttributesToSkip = FileAttributes.System | FileAttributes.Hidden,
        RecurseSubdirectories = false,
        MatchType = MatchType.Simple,
        ReturnSpecialDirectories = false
    };

    private const string SEARCH_PATTERN = "*";

    public CreateHardLinkHandler(CreateHardLinkOption option,
        IOverwriteDisplay? overwriteDisplays = null, int refreshTime = 1000, ILogger? logger = null)
    {
        _option = option;

        _logger = logger ?? new Logger();

        _overwriteDisplays = overwriteDisplays;

        _refreshTime = refreshTime;

        _results = new CreateHardLinkResults();

        _fileChannel = Channel.CreateBounded<FileEntry>(new BoundedChannelOptions(128)
        {
            SingleReader = false,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        });

        _directoryChannel = Channel.CreateBounded<DirectoryEntry>(new BoundedChannelOptions(64)
        {
            SingleReader = false,
            FullMode = BoundedChannelFullMode.Wait
        });
    }

    public async Task<CreateHardLinkResults> RunAsync(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            _results.IsCancel = true;
            return _results;
        }

        bool isCancel = false;
        var refreshToken = CancellationTokenSource.CreateLinkedTokenSource(token);
        Task? progressDisplayTask = null;
        _stopwatch = Stopwatch.StartNew();
        try
        {
            if (IsFile(_option.Target))
            {
                await CreateFileHardLinkAsync(new FileInfo(_option.Target), _option.Output, token)
                    .ConfigureAwait(false);
            }
            else
            {
                if (_overwriteDisplays is not null)
                    progressDisplayTask = RefreshAsync(refreshToken.Token);

                int directoryProcessorCount = Environment.ProcessorCount / 4;
                _directoryProcessor = new Task[directoryProcessorCount];
                for (int i = 0; i < directoryProcessorCount; i++)
                    _directoryProcessor[i] = ProcessDirectoryEntriesAsync(token);

                int fileProcessorCount = Environment.ProcessorCount - directoryProcessorCount;
                _fileProcessor = new Task[fileProcessorCount];
                for (int i = 0; i < fileProcessorCount; i++)
                    _fileProcessor[i] = ProcessFileEntriesAsync(token);

                await ProducesDirectoryEntriesAsync(new DirectoryInfo(_option.Target), _option.Output, token)
                    .ConfigureAwait(false);
                _directoryChannel.Writer.Complete();
                await Task.WhenAll(_directoryProcessor).WaitAsync(token).ConfigureAwait(false);
                _fileChannel.Writer.Complete();
                await Task.WhenAll(_fileProcessor).WaitAsync(token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            isCancel = true;
        }

        await refreshToken.CancelAsync().ConfigureAwait(false);
        if (progressDisplayTask is not null)
            await progressDisplayTask.ConfigureAwait(false);

        _stopwatch.Stop();

        _results.ElapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
        _results.IsCancel = isCancel;
        return _results;
    }

    private async Task ProducesDirectoryEntriesAsync(DirectoryInfo target, string output, CancellationToken token)
    {
        await _directoryChannel.Writer.WriteAsync(new DirectoryEntry(target, output), token).ConfigureAwait(false);
        foreach (var info in target.EnumerateDirectories(SEARCH_PATTERN, _processDirectoryEntriesEnumerationOptions))
        {
            await ProducesDirectoryEntriesAsync(info, Path.Combine(output, info.Name), token).ConfigureAwait(false);
        }
    }

    private async Task ProcessDirectoryEntriesAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await foreach (var entry in _directoryChannel.Reader.ReadAllAsync(token).ConfigureAwait(false))
        {
            try
            {
                Interlocked.Increment(ref _results.TotalDirectory);
                DirectoryInfo info = new DirectoryInfo(entry.Output);
                if (info.Exists)
                {
                    Interlocked.Increment(ref _results.RepetitionDirectory);
                }
                else
                {
                    Interlocked.Increment(ref _results.NewDirectory);
                    info.Create();
                }

                foreach (FileInfo fileInfo in entry.Target.EnumerateFiles(SEARCH_PATTERN,
                             _processFileEntriesEnumerationOptions))
                {
                    await _fileChannel.Writer.WriteAsync(
                            new FileEntry(fileInfo, Path.Combine(entry.Output, fileInfo.Name)), token)
                        .ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.Error($"创建目录 {entry.Target.FullName} -> {entry.Output} 失败: \n{e}");
                Interlocked.Increment(ref _results.FailureDirectory);
            }
        }
    }

    private async Task ProcessFileEntriesAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await foreach (var entry in _fileChannel.Reader.ReadAllAsync(token).ConfigureAwait(false))
        {
            try
            {
                await CreateFileHardLinkAsync(entry.Target, entry.Output, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.Error($"创建文件 {entry.Target.FullName} -> {entry.Output} 硬链接失败: \n{e}");
            }
        }
    }

    private Task CreateFileHardLinkAsync(FileInfo info, string newFullPath, CancellationToken token)
    {
        try
        {
            if (token.IsCancellationRequested)
                return Task.FromCanceled(token);

            Interlocked.Increment(ref _results.TotalFile);
            if (_option.SkipSize > info.Length)
            {
                FileSystem.CopyFile(info.FullName, newFullPath, _option.IsOverwrite);
                Interlocked.Increment(ref _results.SkipFile);
                return Task.CompletedTask;
            }

            if (File.Exists(newFullPath))
            {
                if (!_option.IsOverwrite)
                {
                    Interlocked.Increment(ref _results.RepetitionFile);
                    return Task.CompletedTask;
                }

                Interlocked.Increment(ref _results.OverwriteFile);
                FileSystem.DeleteFile(newFullPath);
            }

            if (TryCreateHardLink(info.FullName, newFullPath))
            {
                Interlocked.Increment(ref _results.SuccessFile);
            }
            else
            {
                _logger.Error($"创建 {info.FullName} 至 {newFullPath} 硬链接失败");
                Interlocked.Increment(ref _results.FailureFile);
            }

            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            Interlocked.Increment(ref _results.FailureFile);
            return Task.FromException(e);
        }
    }

    private async Task RefreshAsync(CancellationToken token)
    {
        try
        {
            if (_overwriteDisplays is null) return;
            while (!token.IsCancellationRequested)
            {
                _overwriteDisplays.Overwrite
                (
                    $"成功 {_results.SuccessFile} 个文件. " + $"失败 {_results.FailureFile} 个文件. \n" +
                    $"直接复制 {_results.SkipFile} 个文件. 已存在 {_results.RepetitionFile} 个文件. 覆盖 {_results.OverwriteFile} 个文件. \n" +
                    $"总共 {_results.TotalFile} 个文件. \n\n" +
                    $"新建 {_results.NewDirectory} 个文件夹. " + $"无法新建 {_results.FailureDirectory} 个文件夹. \n" +
                    $"已存在 {_results.RepetitionDirectory} 个文件夹. 覆盖 {_results.OverwriteDirectory} 个文件夹. \n" +
                    $"总共 {_results.TotalDirectory} 个文件夹. \n\n" +
                    $"总共耗时 {_stopwatch?.ElapsedMilliseconds ?? -1L} 毫秒. \n" +
                    $"总共 {_results.TotalFile + _results.TotalDirectory} 个文件/文件夹. \n"
                );
                await Task.Delay(_refreshTime, token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
        finally
        {
            _overwriteDisplays?.Repetition();
        }
    }
}