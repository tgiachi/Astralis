using Astralis.Core.Extensions;
using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Extensions;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Core.Server.Services;
using Astralis.Core.Services;
using Astralis.Game.Client.Core;
using Astralis.Game.Client.Data;
using Astralis.Game.Client.Impl;
using Astralis.Game.Client.Interfaces.Services;
using Astralis.Game.Client.Modules;
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
    private static IOpenGlContext _openGlContext = null!;
    public static async Task Main(string[] args)
    {

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .MinimumLevel.Debug()
            .CreateLogger();

        Log.Logger.Information("View thread id: {ThreadId}", Environment.CurrentManagedThreadId);

        var builder = Host.CreateApplicationBuilder(args);

        builder.Logging.ClearProviders().AddSerilog();


        builder.Services.AddContainerModule<EcsModule>();
        builder.Services
            .AddSingleton<IEventBusService, EventBusService>()
            .AddSingleton<INetworkClient, NetworkClient>()
            .AddSingleton<ITextureManagerService, TextureManagerService>()
            .AddSingleton(provider => _openGlContext)
            .AddSingleton(new AstralisGameConfig());
//            .AddSingleton<IOpenGlContext, OpenGlContext>();


        builder.Services
            .AddSystemService<IVariablesService, VariablesService>()
            .AddSystemService<IVersionService, VersionService>(10);

        builder.Services.AddHostedService<AstralisGameClient>();

        Log.Logger.Information("Astralis Game Client starting up...");


        var app = builder.Build();
        _openGlContext = new OpenGlContext(new AstralisGameConfig(), app.Services.GetService<IEventBusService>());
        app.RunAsync();
    }
}
