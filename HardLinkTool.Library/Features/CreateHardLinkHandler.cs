using System.Threading.Channels;
using HardLinkTool.Library.Enums;
using HardLinkTool.Library.Features.Loggers;
using HardLinkTool.Library.Features.ResultUpdate;
using HardLinkTool.Library.Modules;
using Microsoft.VisualBasic.FileIO;
using static HardLinkTool.Library.Utils.CreateHardLinkUtils;

namespace HardLinkTool.Library.Features;

public sealed class CreateHardLinkHandler
{
    private readonly ILogger _logger;
    private readonly IProgressReport? _progressReport;
    private CreateHardLinkResults _results;

    public CreateHardLinkOption Option { get; private set; }

    public CreateHardLinkResults Results => _results;

    public bool IsRunning { get; private set; }

    private readonly Channel<FileEntry> _fileChannel = Channel.CreateBounded<FileEntry>(new BoundedChannelOptions(128)
    {
        SingleReader = false,
        SingleWriter = false,
        FullMode = BoundedChannelFullMode.Wait
    });

    private readonly Channel<DirectoryEntry> _directoryChannel = Channel.CreateBounded<DirectoryEntry>(
        new BoundedChannelOptions(64)
        {
            SingleReader = false,
            FullMode = BoundedChannelFullMode.Wait
        });

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

    public CreateHardLinkHandler(ILogger logger, IProgressReport? progressReport)
    {
        _logger = logger;
        _progressReport = progressReport;
    }

    public async Task<CreateHardLinkResults> RunAsync(CreateHardLinkOption option, CancellationToken token = default)
    {
        if (IsRunning)
            throw new InvalidOperationException("Already running.");

        IsRunning = true;
        Option = option;
        _results = new CreateHardLinkResults();
        if (token.IsCancellationRequested)
        {
            _results.state = CreateHardLinkState.Canceled;
            return Results;
        }

        var refreshToken = CancellationTokenSource.CreateLinkedTokenSource(token);
        Task? progressDisplayTask = null;

        Results.Stopwatch.Start();
        try
        {
            if (Option.HardLinkEntry.All(target => IsFile(target.Target)))
            {
                try
                {
                    foreach (HardLinkEntry hardLinkEntry in Option.HardLinkEntry)
                    {
                        await CreateFileHardLinkAsync(new FileInfo(hardLinkEntry.Target), hardLinkEntry.Output, token)
                            .ConfigureAwait(false);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }
            }
            else
            {
                if (_progressReport is not null)
                    progressDisplayTask = RefreshAsync(_progressReport, refreshToken.Token);

                int directoryProcessorCount = Environment.ProcessorCount / 4;
                Task[] directoryProcessor = new Task[directoryProcessorCount];
                for (int i = 0; i < directoryProcessorCount; i++)
                    directoryProcessor[i] = ProcessDirectoryEntriesAsync(token);

                int fileProcessorCount = Environment.ProcessorCount - directoryProcessorCount;
                Task[] fileProcessor = new Task[fileProcessorCount];
                for (int i = 0; i < fileProcessorCount; i++)
                    fileProcessor[i] = ProcessFileEntriesAsync(token);

                await ProducesEntriesAsync(token)
                    .ConfigureAwait(false);
                _directoryChannel.Writer.Complete();
                await Task.WhenAll(directoryProcessor).WaitAsync(token).ConfigureAwait(false);
                _fileChannel.Writer.Complete();
                await Task.WhenAll(fileProcessor).WaitAsync(token).ConfigureAwait(false);
            }

            _results.state = CreateHardLinkState.Completed;
        }
        catch (OperationCanceledException)
        {
            _results.state = CreateHardLinkState.Canceled;
            return _results;
        }
        catch (Exception e)
        {
            _results.exception = e;
            _results.state = CreateHardLinkState.Failed;
            return _results;
        }
        finally
        {
            _results.Stopwatch.Stop();
            _progressReport?.Complete(Results);
            await refreshToken.CancelAsync().ConfigureAwait(false);
            if (progressDisplayTask is not null)
                await progressDisplayTask.ConfigureAwait(false);
            refreshToken.Dispose();
            IsRunning = false;
        }

        return _results;
    }


    private async Task ProducesEntriesAsync(CancellationToken token)
    {
        var queue = new Queue<DirectoryEntry>(256);
        foreach (HardLinkEntry hardLinkEntry in Option.HardLinkEntry)
        {
            if (IsFile(hardLinkEntry.Target))
            {
                await _fileChannel.Writer.WriteAsync(
                    new FileEntry(new FileInfo(hardLinkEntry.Target), hardLinkEntry.Output), token);
            }
            else
            {
                queue.Enqueue(new DirectoryEntry(new DirectoryInfo(hardLinkEntry.Target), hardLinkEntry.Output));
            }
        }

        while (queue.Count > 0)
        {
            DirectoryEntry entry = queue.Dequeue();
            await _directoryChannel.Writer.WriteAsync(entry, token).ConfigureAwait(false);

            foreach (var info in entry.Target.EnumerateDirectories(SEARCH_PATTERN,
                         _processDirectoryEntriesEnumerationOptions))
            {
                string newOutputPath = Path.Combine(entry.Output, info.Name);
                queue.Enqueue(new DirectoryEntry(info, newOutputPath));
            }
        }
    }

    private async Task ProcessDirectoryEntriesAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await foreach (var entry in _directoryChannel.Reader.ReadAllAsync(token).ConfigureAwait(false))
        {
            try
            {
                Interlocked.Increment(ref _results.totalDirectory);
                DirectoryInfo info = new DirectoryInfo(entry.Output);
                if (info.Exists)
                {
                    Interlocked.Increment(ref _results.repetitionDirectory);
                }
                else
                {
                    Interlocked.Increment(ref _results.newDirectory);
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
                Interlocked.Increment(ref _results.failureDirectory);
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

    private async Task CreateFileHardLinkAsync(FileInfo info, string newFullPath, CancellationToken token)
    {
        try
        {
            token.ThrowIfCancellationRequested();

            Interlocked.Increment(ref _results.totalFile);
            if (File.Exists(newFullPath))
            {
                if (!Option.IsOverwrite)
                {
                    Interlocked.Increment(ref _results.repetitionFile);
                    return;
                }

                Interlocked.Increment(ref _results.overwriteFile);
            }

            if (Option.SkipSize > 0L && Option.SkipSize > info.Length)
            {
                if (info.Length > 1024L * 1024L * 10L)
                {
                    await using FileStream originalStream = info.OpenRead();
                    await using FileStream targetStream =
                        new FileStream(newFullPath, FileMode.Create, FileAccess.Write);
                    await originalStream.CopyToAsync(targetStream, token).ConfigureAwait(false);
                }
                else
                {
                    info.CopyTo(newFullPath, true);
                }

                Interlocked.Increment(ref _results.successFile);
                return;
            }

            if (File.Exists(newFullPath))
                FileSystem.DeleteFile(newFullPath);


            if (TryCreateHardLink(info.FullName, newFullPath))
            {
                Interlocked.Increment(ref _results.successFile);
            }
            else
            {
                _logger.Error($"创建 {info.FullName} 至 {newFullPath} 硬链接失败");
                Interlocked.Increment(ref _results.failureFile);
            }
        }
        catch (Exception)
        {
            Interlocked.Increment(ref _results.failureFile);
            throw;
        }
    }

    private async Task RefreshAsync(IProgressReport progressReport, CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(progressReport.UpdateInterval, token).ConfigureAwait(false);
                progressReport.Report(Results);
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
    }
}
