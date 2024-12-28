using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Game.Client.Interfaces.Services;

namespace Astralis.Game.Client;

public static class AstralisGameInstances
{
    public static AstralisServiceProvider ServiceProvider { get; set; }

    public static IVariablesService VariablesService() => ServiceProvider.GetService<IVariablesService>();
    public static IVersionService VersionService() => ServiceProvider.GetService<IVersionService>();
    public static IEcsService EcsService() => ServiceProvider.GetService<IEcsService>();
    public static IEventBusService EventBusService() => ServiceProvider.GetService<IEventBusService>();
    public static IOpenGlContext OpenGlContext { get; set; }
}
