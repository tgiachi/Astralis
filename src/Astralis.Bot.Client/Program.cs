using Astralis.Core.Interfaces.Services;
using Astralis.Core.Services;
using Astralis.Network;
using Astralis.Network.Client.Interfaces;
using Astralis.Network.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Astralis.Bot.Client;

public class Program
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Debug().CreateLogger();


        var builder = Host.CreateApplicationBuilder(args);


        builder.Logging.ClearProviders().AddSerilog();


        builder.Services.AddSingleton(MessageTypesInstance.MessageTypesList);

        builder.Services
            .AddSingleton<IEventBusService, EventBusService>()
            .AddSingleton<INetworkClient, NetworkClient>()
            .AddHostedService<ClientHostedService>();

        Log.Logger.Information("Astralis Bot Client starting up...");


        var app = builder.Build();


        await app.RunAsync();
    }
}
