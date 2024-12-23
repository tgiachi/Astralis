using Astralis.Core.Extensions;
using Astralis.Core.Server.Data.Config;
using Astralis.Core.Server.Data.Directories;
using Astralis.Core.Server.Extensions;
using Astralis.Core.Server.Types;
using Astralis.Core.Server.Utils;
using Astralis.Server.Data.Configs;
using Astralis.Server.Extensions;
using Astralis.Server.Impl;
using Astralis.Server.Modules;
using Astralis.Server.Services.Modules;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;

namespace Astralis.Server;

class Program
{
    private static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var options = Parser.Default.ParseArguments<AstralisRootOptions>(args).Value;

        if (string.IsNullOrEmpty(options.RootPath))
        {
            options.RootPath = Path.Combine(Directory.GetCurrentDirectory(), "orion");
        }

        var directoriesConfig = new DirectoriesConfig(options.RootPath);

        if (Environment.GetEnvironmentVariable("ORION_ROOT_PATH") is { } envRootPath)
        {
            directoriesConfig = new DirectoriesConfig(envRootPath);
        }


        var logConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(options.LogLevel.ToLogLevel())
            .WriteTo.Console();


        builder.Services.AddSingleton(options);

        if (options.LogOnFile)
        {
            logConfiguration.WriteTo.File(
                new CompactJsonFormatter(),
                Path.Combine(directoriesConfig[DirectoryType.Logs], "orion_server.log"),
                rollingInterval: RollingInterval.Day
            );
        }

        Log.Logger = logConfiguration.CreateLogger();

        Log.Logger.Information("> Root path: {RootPath}", directoriesConfig.Root);
        Log.Logger.Information("> Log level: {LogLevel}", options.LogLevel);
        Log.Logger.Information("> Is docker: {IsDocker}", EnvUtils.IsOnDocker);

        builder.Logging.ClearProviders().AddSerilog();

        builder.Services
            .AddSingleton(directoriesConfig)
            .AddSingleton(new ProcessQueueConfig(options.MaxConcurrentProcess))
            .AddSingleton(new NetworkServerConfig(options.ServerPort))
            .AddSingleton(new HttpServerConfig(options.HttpPort))
            .AddSingleton(new SchedulerServiceConfig(20, 15, 4));


        builder.Services.RegisterDatabaseAndScanEntities(options.DatabaseType);

        builder.Services
            .AddContainerModule<BaseServiceModule>()
            .AddContainerModule<JsonModule>()
            .AddContainerModule<ScriptModule>()
            .AddContainerModule<NetworkServiceModule>();


        builder.Services.AddContainerModule<GameServicesModule>();

        builder.Services.AddHostedService<AstralisServerManager>();

        var app = builder.Build();

        await app.RunAsync();
    }
}
