using Astralis.Core.Server.Attributes.Scripts;
using Serilog;

namespace Astralis.Server.Scripts;

[ScriptModule]
public class LoggerModule
{
    private readonly ILogger _logger = Log.ForContext<LoggerModule>();


    [ScriptFunction("log_info")]
    public void LogInfo(string message)
    {
        _logger.Information(message);
    }

    [ScriptFunction("log_debug")]
    public void LogDebug(string message)
    {
        _logger.Debug(message);
    }

    [ScriptFunction("log_warning")]
    public void LogWarning(string message)
    {
        _logger.Warning(message);
    }

    [ScriptFunction("log_error")]
    public void LogError(string message)
    {
        _logger.Error(message);
    }
}
