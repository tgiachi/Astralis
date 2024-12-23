using Astralis.Core.Server.Types;
using Serilog.Events;

namespace Astralis.Core.Server.Extensions;

public static class OrionLogLevelExtension
{
    public static LogEventLevel ToLogLevel(this AstralisLogLevel logLevel)
    {
        return logLevel switch
        {
            AstralisLogLevel.Trace       => LogEventLevel.Verbose,
            AstralisLogLevel.Debug       => LogEventLevel.Debug,
            AstralisLogLevel.Information => LogEventLevel.Information,
            AstralisLogLevel.Warning     => LogEventLevel.Warning,
            AstralisLogLevel.Error       => LogEventLevel.Error,
            AstralisLogLevel.Critical    => LogEventLevel.Fatal,
            _                         => LogEventLevel.Information
        };
    }
}
