using HardLinkTool.Library.Features.ResultUpdate;
using HardLinkTool.Library.Modules;

namespace HardLinkTool.ConsoleApp;

public class OverwriteProgressReport : IProgressReport
{
    public int UpdateInterval { get; }
    private int _messageCount;
    private bool _isAlternate;

    public void Report(CreateHardLinkResults results)
    {
        if (!_isAlternate)
        {
            _isAlternate = true;
            Console.Write("\e[?1049h");
            Console.SetCursorPosition(0, 0);
        }

        Console.SetCursorPosition(0, 0);
        string message = $"成功 {results.SuccessFile} 个文件. " + $"失败 {results.FailureFile} 个文件. \n" +
                         $"直接复制 {results.SkipFile} 个文件. 已存在 {results.RepetitionFile} 个文件. 覆盖 {results.OverwriteFile} 个文件. \n" +
                         $"总共 {results.TotalFile} 个文件. \n\n" +
                         $"新建 {results.NewDirectory} 个文件夹. " + $"无法新建 {results.FailureDirectory} 个文件夹. \n" +
                         $"已存在 {results.RepetitionDirectory} 个文件夹. 覆盖 {results.OverwriteDirectory} 个文件夹. \n" +
                         $"总共 {results.TotalDirectory} 个文件夹. \n\n" +
                         $"总共耗时 {results.ElapsedMilliseconds} 毫秒. \n" +
                         $"总共 {results.TotalFile + results.TotalDirectory} 个文件/文件夹. \n";
        Console.Write(message.Length < _messageCount ? new string(' ', _messageCount - message.Length) : message);
        _messageCount = message.Length;
    }

    public void Complete(CreateHardLinkResults results)
    {
        if (!_isAlternate) return;
        Console.Write("\e[?1049l");
        _isAlternate = false;
    }

    public OverwriteProgressReport(int updateInterval)
    {
        UpdateInterval = updateInterval;
    }
}