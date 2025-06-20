using System.CommandLine;
using System.Diagnostics;

namespace HardLinkTool;

public static class Program
{
    public const string HAND_LINK_POSTFIX = "-link";

    public static async Task<int> Main(string[] args)
    {
        var pathArgument = new Argument<string>("Input", "源文件/文件夹路径.");
        var outputOption = new Option<string?>(["--output", "-o"], "输出文件路径.") { Name = "Output" };
        var skipSizeOption = new Option<long>(["--skipSize", "-s"], () => 1024L, "直接复制文件大小") { Name = "SkipSize" };
        var overwriteOption = new Option<bool>(["--overwrite", "-r"], "是否覆盖已存在的文件.") { Name = "IsOverwrite" };

        var rootCommand = new RootCommand("批量生成硬链接工具.")
        {
            pathArgument,
            outputOption,
            skipSizeOption,
            overwriteOption,
        };
        rootCommand.SetHandler(async option =>
            {
                var handler = new CreateHardLinkHandler(option.Input,
                    option.Output, option.SkipSize, option.IsOverwrite);
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

                    return;
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
            },
            new CreateHardLinkBinder(pathArgument, outputOption, skipSizeOption, overwriteOption));


        return await rootCommand.InvokeAsync(args);
    }
}