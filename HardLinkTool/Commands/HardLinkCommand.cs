using System.CommandLine;
using System.Runtime.InteropServices;
using HardLinkTool.Features;
using HardLinkTool.Features.Enums;
using HardLinkTool.Features.Interfaces;
using HardLinkTool.Features.LoggerDisplays;
using HardLinkTool.Modules;

namespace HardLinkTool.Commands;

public class HardLinkCommand : RootCommand
{
    private readonly Argument<string> _pathArgument = new("Input") { Description = "源文件/文件夹路径." };

    private readonly Option<string?> _outputOption = new("Output", "--output", "-o")
        { Description = "输出文件路径." };

    private readonly Option<long> _skipSizeOption = new("SkipSize", "--skipSize", "-s")
        { Description = "直接复制文件大小", DefaultValueFactory = _ => 1024L };

    private readonly Option<bool> _noProgressOption = new("Progress", "--no-progress", "-np")
        { Description = "是否不显示进度." };

    private readonly Option<int> _refreshTimeOption = new("RefreshTime", "--refresh")
        { Description = "进度刷新时间", DefaultValueFactory = _ => 1000 };

    private readonly Option<bool> _overwriteOption = new("IsOverwrite", "--overwrite", "-r")
        { Description = "是否覆盖已存在的文件." };
    
    public HardLinkCommand() :
        base("批量生成硬链接工具.")
    {
        this.Add(_pathArgument);
        this.Add(_outputOption);
        this.Add(_skipSizeOption);
        this.Add(_noProgressOption);
        this.Add(_refreshTimeOption);
        this.Add(_overwriteOption);

        this.SetAction(async parse =>
        {
            ILogger logger = new Logger().AddDebugDisplay(new ConsoleDisplay(LoggerLevel.Debug))
                .AddInfoDisplay(new ConsoleDisplay(LoggerLevel.Info))
                .AddWarnDisplay(new ConsoleDisplay(LoggerLevel.Warn))
                .AddErrorDisplay(new LocalFileDisplay(LoggerLevel.Error, @".\error.log"))
                .AddFatalDisplay(new ConsoleDisplay(LoggerLevel.Fatal));

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                logger.Fatal("本软件仅支持 Windows 系统!");
                return (int)ErrorCode.NotSupported;
            }

            string path = parse.GetRequiredValue(_pathArgument);
            string? outputPath = parse.GetValue(_outputOption);
            long skipSize = parse.GetValue(_skipSizeOption);
            bool isOverwrite = parse.GetValue(_overwriteOption);
            OverwriteDisplay? overwriteDisplay = !parse.GetValue(_noProgressOption) ? new OverwriteDisplay() : null;
            int refreshTime = parse.GetValue(_refreshTimeOption);
            var handler = new CreateHardLinkHandler(path, outputPath, skipSize, isOverwrite,
                overwriteDisplay, refreshTime, logger);
            CreateHardLinkResults result;
            try
            {
                result = await handler.RunAsync();
            }
            catch (Exception e)
            {
                logger.Fatal($"出现错误: 根目录下已输出错误日志 \nMessage: {e.Message}");
                logger.Error($"\n{e}");
                return (int)ErrorCode.Error;
            }

            logger.Info($"\n成功 {result.SuccessFile} 个文件. " + $"失败 {result.FailureFile} 个文件. \n" +
                        $"直接复制 {result.SkipFile} 个文件. 已存在 {result.RepetitionFile} 个文件. 覆盖 {result.OverwriteFile} 个文件. \n" +
                        $"总共 {result.TotalFile} 个文件. \n\n" +
                        $"新建 {result.NewDirectory} 个文件夹. " + $"无法新建 {result.FailureDirectory} 个文件夹. \n" +
                        $"已存在 {result.RepetitionDirectory} 个文件夹. 覆盖 {result.OverwriteDirectory} 个文件夹. \n" +
                        $"总共 {result.TotalDirectory} 个文件夹. \n\n" +
                        $"总共耗时 {result.ElapsedMilliseconds} 毫秒. \n" +
                        $"总共 {result.TotalFile + result.TotalDirectory} 个文件/文件夹. \n" +
                        $"{(result.SuccessFile == 0 ? "未能输出任何文件! " : $"输出在: {handler.Output}")} \n");
            if (result.FailureFile > 0 || result.FailureDirectory > 0)
                logger.Warn("部分任务失败, 请查看错误日志.");

            return (int)ErrorCode.Ok;
        });
    }
}