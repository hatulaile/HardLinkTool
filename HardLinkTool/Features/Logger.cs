using HardLinkTool.Features.Interfaces;
using HardLinkTool.Features.LoggerDisplays;
using HardLinkTool.Features.Utils;

namespace HardLinkTool.Features;

public class Logger : ILogger
{
    private readonly List<ILoggerDisplay> _debugDisplays = [];
    private readonly List<ILoggerDisplay> _infoDisplays = [];
    private readonly List<ILoggerDisplay> _warnDisplays = [];
    private readonly List<ILoggerDisplay> _errorDisplays = [];
    private readonly List<ILoggerDisplay> _fatalDisplays = [];

    public void Debug(object message)
    {
        foreach (ILoggerDisplay loggerDisplay in _debugDisplays)
            loggerDisplay.Log(message);
    }

    public void Info(object message)
    {
        foreach (ILoggerDisplay loggerDisplay in _infoDisplays)
            loggerDisplay.Log(message);
    }

    public void Warn(object message)
    {
        foreach (ILoggerDisplay loggerDisplay in _warnDisplays)
            loggerDisplay.Log(message);
    }

    public void Error(object message)
    {
        foreach (ILoggerDisplay loggerDisplay in _errorDisplays)
            loggerDisplay.Log(message);
    }

    public void Fatal(object message)
    {
        foreach (ILoggerDisplay loggerDisplay in _fatalDisplays)
            loggerDisplay.Log(message);
    }
    
    public Logger AddDebugDisplay(params ILoggerDisplay[] display)
    {
        _debugDisplays.AddRange(display);
        return this;
    }

    public Logger AddInfoDisplay(params ILoggerDisplay[] display)
    {
        _infoDisplays.AddRange(display);
        return this;
    }

    public Logger AddWarnDisplay(params ILoggerDisplay[] display)
    {
        _warnDisplays.AddRange(display);
        return this;
    }

    public Logger AddErrorDisplay(params ILoggerDisplay[] display)
    {
        _errorDisplays.AddRange(display);
        return this;
    }

    public Logger AddFatalDisplay(params ILoggerDisplay[] display)
    {
        _fatalDisplays.AddRange(display);
        return this;
    }
}