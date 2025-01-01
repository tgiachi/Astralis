using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Game.Client.Core.Visuals;
using Astralis.Game.Client.Data.Config;
using Astralis.Game.Client.Interfaces.Services;

namespace Astralis.Game.Client;

public static class AstralisGameInstances
{
    public static AstralisServiceProvider ServiceProvider { get; set; }

    public static IVariablesService VariablesService() => ServiceProvider.GetService<IVariablesService>();
    public static IVersionService VersionService() => ServiceProvider.GetService<IVersionService>();
    public static IEcsService EcsService() => ServiceProvider.GetService<IEcsService>();
    public static IFontManagerService FontManagerService() => ServiceProvider.GetService<IFontManagerService>();
    public static IEventBusService EventBusService() => ServiceProvider.GetService<IEventBusService>();

    public static Camera Camera { get; set; }
    public static ITextureManagerService TextureManagerService() => ServiceProvider.GetService<ITextureManagerService>();
    public static IOpenGlContext OpenGlContext { get; set; }

    public static AssetDirectories AssetDirectories { get; set; }
}
