using Astralis.Core.Interfaces.Modules;
using Astralis.Core.Server.Extensions;
using Astralis.Server.Services.Handlers;
using Astralis.Server.Services.Impl;
using Astralis.Server.Services.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Astralis.Server.Services.Modules;

public class GameServicesModule : IContainerModule
{
    public IServiceCollection RegisterServices(ILogger logger, IServiceCollection services)
    {
        services.AddGameService<IWorldService, WorldService>();
        return services
            .AddHandlerService<PlayerHandler>()
            .AddHandlerService<MotdHandler>()
            .AddHandlerService<WorldHandler>();
    }
}
