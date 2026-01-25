namespace HardLinkTool.UI.Services;

public interface ICreateHardLinkConfig
{
    long SkipSize { get; set;}
    
    bool IsOverwrite { get; set; }
}