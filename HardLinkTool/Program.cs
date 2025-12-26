using System.CommandLine;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HardLinkTool.Features;
using HardLinkTool.Features.Enums;
using HardLinkTool.Features.Interfaces;
using HardLinkTool.Features.LoggerDisplays;
using HardLinkTool.Modules;

namespace HardLinkTool;

public static class Program
{
    public const string HAND_LINK_POSTFIX = "-link";

    public static async Task<int> Main(string[] args)
    {
        var pathArgument = new Argument<string>("Input") { Description = "源文件/文件夹路径." };
        var outputOption = new Option<string?>("Output", ["--output", "-o"]) { Description = "输出文件路径." };
        var skipSizeOption = new Option<long>("SkipSize", ["--skipSize", "-s"])
            { Description = "直接复制文件大小", DefaultValueFactory = _ => 1024L };
        var progressOption = new Option<bool>("Progress", ["--no-progress", "-np"]) { Description = "是否不显示进度." };
        var refreshTimeOption = new Option<int>("RefreshTime", ["--refresh"])
            { Description = "进度刷新时间", DefaultValueFactory = _ => 1000 };
        var overwriteOption = new Option<bool>("IsOverwrite", ["--overwrite", "-r"]) { Description = "是否覆盖已存在的文件." };

        var rootCommand = new RootCommand("批量生成硬链接工具.")
        {
            pathArgument,
            outputOption,
            skipSizeOption,
            progressOption,
            refreshTimeOption,
            overwriteOption,
        };
        rootCommand.SetAction(async parse =>
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


            var handler = new CreateHardLinkHandler(parse.GetRequiredValue(pathArgument), parse.GetValue(outputOption),
                parse.GetValue(skipSizeOption), parse.GetValue(overwriteOption), logger,
                !parse.GetValue(progressOption) ? new OverwriteDisplay() : null, parse.GetValue(refreshTimeOption));
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


        return await rootCommand.Parse(args).InvokeAsync();
    }
}