namespace HardLinkTool.Features.Interfaces;

public interface ILogger
{
    void Debug(object message);
    
    void Info(object message);

    void Warn(object message);
    
    void Error(object message);
    
    void Fatal(object message);
}