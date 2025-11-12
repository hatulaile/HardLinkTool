using System.CommandLine;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
        var overwriteOption = new Option<bool>("IsOverwrite", ["--overwrite", "-r"]) { Description = "是否覆盖已存在的文件." };

        var rootCommand = new RootCommand("批量生成硬链接工具.")
        {
            pathArgument,
            outputOption,
            skipSizeOption,
            overwriteOption,
        };
        rootCommand.SetAction(async parse =>
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                await Console.Error.WriteLineAsync("本软件仅支持 Windows 系统!");
                return (int)ErrorCode.NotSupported;
            }

            var handler = new CreateHardLinkHandler(parse.GetRequiredValue(pathArgument), parse.GetValue(outputOption),
                parse.GetValue(skipSizeOption), parse.GetValue(overwriteOption));
            CreateHardLinkResults result;
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                result = await handler.RunAsync();
            }
            catch (Exception e)
            {
#if DEBUG
                    await Console.Error.WriteLineAsync($"遇到错误! \n信息: {e}");
#else
                await Console.Error.WriteLineAsync($"错误: {e.Message}");
#endif

                return (int)ErrorCode.Error;
            }

            stopwatch.Stop();
            await Console.Out.WriteLineAsync($"成功 {result.SuccessFile} 个文件. " + $"失败 {result.FailureFile} 个文件. \n" +
                                             $"直接复制 {result.SkipFile} 个文件. 已存在 {result.RepetitionFile} 个文件. 覆盖 {result.OverwriteFile} 个文件. \n" +
                                             $"总共 {result.TotalFile} 个文件. \n\n" +
                                             $"新建 {result.NewDirectory} 个文件夹. " +
                                             $"无法新建 {result.FailureDirectory} 个文件夹. \n" +
                                             $"已存在 {result.RepetitionDirectory} 个文件夹. 覆盖 {result.OverwriteDirectory} 个文件夹. \n" +
                                             $"总共 {result.TotalDirectory} 个文件夹. \n\n" +
                                             $"总共耗时 {stopwatch.ElapsedMilliseconds} 毫秒. \n" +
                                             $"总共 {result.TotalFile + result.TotalDirectory} 个文件/文件夹. \n" +
                                             $"{(result.SuccessFile == 0 ? "未能输出任何文件! " : $"输出在: {handler.Output}")} \n");
            return (int)ErrorCode.Ok;
        });


        return await rootCommand.Parse(args).InvokeAsync();
    }
}