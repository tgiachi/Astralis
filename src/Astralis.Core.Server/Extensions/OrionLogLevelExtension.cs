using Astralis.Core.Server.Types;
using Serilog.Events;

namespace Astralis.Core.Server.Extensions;

public static class OrionLogLevelExtension
{
    public static LogEventLevel ToLogLevel(this OrionLogLevel logLevel)
    {
        return logLevel switch
        {
            OrionLogLevel.Trace       => LogEventLevel.Verbose,
            OrionLogLevel.Debug       => LogEventLevel.Debug,
            OrionLogLevel.Information => LogEventLevel.Information,
            OrionLogLevel.Warning     => LogEventLevel.Warning,
            OrionLogLevel.Error       => LogEventLevel.Error,
            OrionLogLevel.Critical    => LogEventLevel.Fatal,
            _                         => LogEventLevel.Information
        };
    }
}
