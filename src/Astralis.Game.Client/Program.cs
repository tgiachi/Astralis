﻿using Astralis.Game.Client.Data;
using Astralis.Game.Client.Impl;
using Serilog;


namespace Astralis.Game.Client;

class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .MinimumLevel.Debug()
            .CreateLogger();

        Log.Logger.Information("View thread id: {ThreadId}", Environment.CurrentManagedThreadId);

        AstralisGameInstances.ServiceProvider = new AstralisServiceProvider();

        AstralisGameInstances.VariablesService();
        AstralisGameInstances.VersionService();
        AstralisGameInstances.EcsService();

        AstralisGameInstances.OpenGlContext = new OpenGlContext(
            new AstralisGameConfig(),
            AstralisGameInstances.EventBusService()
        );

        await Task.Run(
            () => { AstralisGameInstances.OpenGlContext.Run(); }
        );
    }
}
