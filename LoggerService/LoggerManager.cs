using Contracts;
using Serilog;

namespace LoggerService;

public class LoggerManager : ILoggerManager
{
    private readonly ILogger _logger = Log.Logger;
    
    public void LogDebug(string message) => _logger.Debug(message);
    public void LogError(string message) => _logger.Error(message);
    public void LogInfo(string message) => _logger.Information(message);
    public void LogWarn(string message) => _logger.Warning(message);
}