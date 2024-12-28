using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Events.Variables;
using Astralis.Core.Server.Interfaces.Services.System;

namespace Astralis.Core.Server.Services;

public class VersionService : IVersionService
{
    public VersionService(IEventBusService eventBusService)
    {
        eventBusService.Publish(new AddVariableEvent("app_version", GetVersion()));
    }

    public string GetVersion()
    {
        var assembly = typeof(VersionService).Assembly;
        var version = assembly.GetName().Version;

        return version.ToString();
    }
}
