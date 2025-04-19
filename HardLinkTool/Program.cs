using System.CommandLine;

namespace HardLinkTool;

public static class Program
{
    const string HAND_LINK_POSTFIX = "-link";

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
                CreateHardLinkResults result;
                string output = option.Output ?? $"{option.Input}{HAND_LINK_POSTFIX}";
                try
                {
                    result = await new CreateHardLinkHandler(option.Input,
                            output, option.SkipSize, option.IsOverwrite)
                        .RunAsync();
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

                await Console.Out.WriteLineAsync($"成功 {result.Success} 个文件. \n" +
                                                 $"失败 {result.Failure} 个文件. \n" +
                                                 $"跳过 {result.Skip} 个文件. \n" +
                                                 $"已存在 {result.Repetition} 个文件. \n" +
                                                 $"总共 {result.Total} 个文件. \n" +
                                                 $"{(result.Success == 0 ? "未能输出任何文件! " : $"输出在: {output}! ")} \n");
            },
            new CreateHardLinkBinder(pathArgument, outputOption, skipSizeOption, overwriteOption));

        return await rootCommand.InvokeAsync(args);
    }
}