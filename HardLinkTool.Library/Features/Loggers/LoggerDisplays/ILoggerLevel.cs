using HardLinkTool.Library.Enums;

namespace HardLinkTool.Library.Features.Loggers.LoggerDisplays;

public interface ILoggerLevel
{
    LoggerLevel Level { get; internal set; }
}