namespace HardLinkTool;

public class CreateHardLinkResults
{
    public CreateHardLinkResults(int success, int failure, int skip, int total)
    {
        Success = success;
        Failure = failure;
        Skip = skip;
        Total = total;
    }

    public int Success { get; init; }

    public int Failure { get; init; }

    public int Skip { get; init; }

    public int Total { get; init; }
}