using System.CommandLine;
using System.Text;
using HardLinkTool.Features;
using HardLinkTool.Features.Enums;
using HardLinkTool.Features.Loggers;
using HardLinkTool.Features.Loggers.LoggerDisplays;
using HardLinkTool.Features.Utils;
using HardLinkTool.Modules;

namespace HardLinkTool.Commands;

public sealed class HardLinkCommand : RootCommand
{
    private readonly Argument<string[]> _pathArgument = new("Targets") { Description = "源文件/文件夹路径." };

    private readonly Option<string[]?> _outputOption = new("Outputs", "--output", "-o")
        { Description = "输出文件路径.", AllowMultipleArgumentsPerToken = true };

    private readonly Option<long> _skipSizeOption = new("SkipSize", "--skipSize", "-s")
        { Description = "直接复制文件大小", DefaultValueFactory = _ => 1024L };

    private readonly Option<bool> _noProgressOption = new("NoProgress", "--no-progress", "-np")
        { Description = "是否不显示进度." };

    private readonly Option<int> _refreshTimeOption = new("RefreshTime", "--refresh")
        { Description = "进度刷新时间", DefaultValueFactory = _ => 1000 };

    private readonly Option<bool> _overwriteOption = new("IsOverwrite", "--overwrite", "-r")
        { Description = "是否覆盖已存在的文件." };

    private readonly Option<bool> _noErrorLogFile = new("NoErrorLogFile", "--no-error-log-file", "-ne")
        { Description = "是否不生成错误日志文件." };


    public HardLinkCommand() :
        base("批量生成硬链接工具.")
    {
        this.Add(_pathArgument);
        this.Add(_outputOption);
        this.Add(_skipSizeOption);
        this.Add(_noProgressOption);
        this.Add(_refreshTimeOption);
        this.Add(_overwriteOption);
        this.Add(_noErrorLogFile);

        this.SetAction(async (parse, token) =>
        {
            Logger logger = new Logger()
                .AddDebugDisplay(new LocalFileDisplay(LoggerLevel.Debug, @".\debug.log"))
                .AddInfoDisplay(new ConsoleDisplay(LoggerLevel.Info))
                .AddWarnDisplay(new ConsoleDisplay(LoggerLevel.Warn))
                .AddFatalDisplay(new ConsoleDisplay(LoggerLevel.Fatal));
            if (!parse.GetValue(_noErrorLogFile))
                logger.AddErrorDisplay(new LocalFileDisplay(LoggerLevel.Error, @".\error.log"));

            HardLinkEntry[] entries = ParseEntries(parse.GetRequiredValue(_pathArgument), parse.GetValue(_outputOption),
                out var errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                logger.Warn(errorMessage);

            if (entries.Length == 0)
            {
                logger.Fatal("没有有效的输入文件.");
                return (int)ErrorCode.Error;
            }

            long skipSize = parse.GetValue(_skipSizeOption);
            if (skipSize < 0) skipSize = 0;

            bool isOverwrite = parse.GetValue(_overwriteOption);
            OverwriteDisplay? overwriteDisplay = !parse.GetValue(_noProgressOption) ? new OverwriteDisplay() : null;
            int refreshTime = parse.GetValue(_refreshTimeOption);


            var handler = new CreateHardLinkHandler(
                new CreateHardLinkOption(entries, skipSize, isOverwrite),
                overwriteDisplay, refreshTime, logger);
            CreateHardLinkResults result;
            try
            {
                result = await handler.RunAsync(token).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.Error($"\n{e}");
                logger.Fatal($"出现错误: 根目录下已输出错误日志 \nMessage: {e.Message}");
                return (int)ErrorCode.Error;
            }

            logger.Info($"\n成功 {result.SuccessFile} 个文件. " + $"失败 {result.FailureFile} 个文件. \n" +
                        $"直接复制 {result.SkipFile} 个文件. 已存在 {result.RepetitionFile} 个文件. 覆盖 {result.OverwriteFile} 个文件. \n" +
                        $"总共 {result.TotalFile} 个文件. \n\n" +
                        $"新建 {result.NewDirectory} 个文件夹. " + $"无法新建 {result.FailureDirectory} 个文件夹. \n" +
                        $"已存在 {result.RepetitionDirectory} 个文件夹. 覆盖 {result.OverwriteDirectory} 个文件夹. \n" +
                        $"总共 {result.TotalDirectory} 个文件夹. \n\n" +
                        $"总共耗时 {result.ElapsedMilliseconds} 毫秒. \n" +
                        $"总共 {result.TotalFile + result.TotalDirectory} 个文件/文件夹. \n");

            if ((result.FailureFile > 0 || result.FailureDirectory > 0) && !parse.GetValue(_noErrorLogFile))
                logger.Warn("部分任务失败, 请查看错误日志.");

            if (result.IsCancel)
            {
                logger.Fatal("任务已中途取消, 未完全复制.");
                return (int)ErrorCode.Cancel;
            }

            return (int)ErrorCode.Ok;
        });
    }

    private static HardLinkEntry[] ParseEntries(string[] targets, string[]? outputs, out string errorMessage)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine();
        List<HardLinkEntry> entries = new List<HardLinkEntry>(targets.Length);
        for (var i = 0; i < targets.Length; i++)
        {
            if (!File.Exists(targets[i]) && !Directory.Exists(targets[i]))
            {
                sb.AppendLine($"输入文件不存在: {targets[i]}");
                continue;
            }

            var entry = new HardLinkEntry(targets[i], ParseOutput(targets[i], outputs?.ElementAtOrDefault(i)));
            if (entry.Target == entry.Output)
            {
                sb.AppendLine($"输出位置与输入文件相同: {entry.Target} -> {entry.Output}");
                continue;
            }

            if (CreateHardLinkUtils.IsEitherParent(entry.Target, entry.Output))
            {
                sb.AppendLine("不能将新位置设置为与目标嵌套的关系!!!! \n" +
                              "如果是未设置输出目录请截图发送 issue!!!! \n" +
                              $"Target :{entry.Target} \n" +
                              $"Output :{entry.Output} \n");
                continue;
            }

            entries.Add(entry);
        }

        sb.Length -= Environment.NewLine.Length;
        errorMessage = sb.ToString();
        return entries.ToArray();
    }

    private static string ParseOutput(string path, string? outPut)
    {
        if (outPut is null)
            return CreateHardLinkUtils.GetDefaultOutput(path, CreateHardLinkUtils.IsFile(path),
                Program.HAND_LINK_POSTFIX);
        else return Path.GetFullPath(outPut);
    }
}