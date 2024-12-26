using Astralis.Core.Interfaces.Services;
using Astralis.Core.Services;
using Astralis.Game.Client.Core;
using Astralis.Game.Client.Data;
using Astralis.Game.Client.Impl;
using Astralis.Game.Client.Interfaces.Services;
using Astralis.Network;
using Astralis.Network.Client.Interfaces;
using Astralis.Network.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Astralis.Game.Client;

class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .MinimumLevel.Debug()
            .CreateLogger();

        var builder = Host.CreateApplicationBuilder(args);

        builder.Logging.ClearProviders().AddSerilog();

        builder.Services
            .AddSingleton<IEventBusService, EventBusService>()
            .AddSingleton<INetworkClient, NetworkClient>()
            .AddSingleton<ITextureManagerService, TextureManagerService>()
            .AddSingleton(new AstralisGameConfig())
            .AddSingleton<IOpenGlContext, OpenGlContext>();

        builder.Services.AddHostedService<AstralisGameClient>();

        Log.Logger.Information("Astralis Game Client starting up...");

        var app = builder.Build();

        await app.RunAsync();
    }
}
