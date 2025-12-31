namespace HardLinkTool.Modules;

public class CreateHardLinkResults
{
    public bool IsCancel;

    public int SuccessFile;

    public int FailureFile;

    public int SkipFile;

    public int RepetitionFile;

    public int OverwriteFile;

    public int TotalFile;

    public int NewDirectory;

    public int FailureDirectory;

    public int RepetitionDirectory;

    public int OverwriteDirectory;

    public int TotalDirectory;

    public long ElapsedMilliseconds;
}