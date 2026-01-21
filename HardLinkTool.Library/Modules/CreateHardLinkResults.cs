using System.Diagnostics;
using System.Runtime.InteropServices;
using HardLinkTool.Library.Enums;

namespace HardLinkTool.Library.Modules;

[StructLayout(LayoutKind.Auto)]
public struct CreateHardLinkResults
{
    internal Exception? exception;
    public Exception? Exception => exception;
    
    internal CreateHardLinkState state;
    public CreateHardLinkState State => state;

    internal int successFile;
    public int SuccessFile => successFile;

    internal int failureFile;
    public int FailureFile => failureFile;

    internal int skipFile;
    public int SkipFile => skipFile;

    internal int repetitionFile;
    public int RepetitionFile => repetitionFile;

    internal int overwriteFile;
    public int OverwriteFile => overwriteFile;

    internal int totalFile;
    public int TotalFile => totalFile;

    internal int newDirectory;
    public int NewDirectory => newDirectory;

    internal int failureDirectory;
    public int FailureDirectory => failureDirectory;

    internal int repetitionDirectory;
    public int RepetitionDirectory => repetitionDirectory;

    internal int overwriteDirectory;
    public int OverwriteDirectory => overwriteDirectory;

    internal int totalDirectory;
    public int TotalDirectory => totalDirectory;

    public long ElapsedMilliseconds => Stopwatch.ElapsedMilliseconds;

    internal readonly Stopwatch stopwatch;
    public Stopwatch Stopwatch => stopwatch;

    public CreateHardLinkResults()
    {
        stopwatch = new Stopwatch();
    }
}