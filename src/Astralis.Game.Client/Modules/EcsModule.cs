using Arch.Core;
using Astralis.Core.Interfaces.Modules;
using Astralis.Core.Server.Extensions;
using Astralis.Game.Client.Impl;
using Astralis.Game.Client.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Astralis.Game.Client.Modules;

public class EcsModule : IContainerModule
{
    public IServiceCollection RegisterServices(ILogger logger, IServiceCollection services)
    {
        return services
            .AddSystemService<IEcsService, EcsService>()
            .AddSingleton(World.Create());
    }
}
