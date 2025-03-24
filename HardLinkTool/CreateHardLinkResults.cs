namespace HardLinkTool;

public struct CreateHardLinkResults
{
    public CreateHardLinkResults(int success, int failure, int skip,int repetition, int total)
    {
        Success = success;
        Failure = failure;
        Skip = skip;
        Repetition = repetition;
        Total = total;
    }

    public int Success { get; init; }

    public int Failure { get; init; }

    public int Skip { get; init; }
    
    public int Repetition { get; init; }

    public int Total { get; init; }
}