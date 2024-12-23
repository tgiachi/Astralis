using Astralis.Core.Interfaces.Modules;
using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Extensions;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Core.Services;
using Astralis.Server.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Astralis.Server.Modules;

public class BaseServiceModule : IContainerModule
{
    public IServiceCollection RegisterServices(ILogger logger, IServiceCollection services)
    {
        services
            .AddSingleton<IEventBusService, EventBusService>()
            .AddSystemService<IVariablesService, VariablesService>()
            .AddSystemService<IEventDispatcherService, EventDispatcherService>()
            .AddSystemService<ISchedulerSystemService, SchedulerSystemService>()
            .AddSystemService<IDiagnosticSystemService, DiagnosticSystemService>()
            .AddSystemService<IProcessQueueService, ProcessQueueService>()
            .AddSystemService<IHttpServerService, HttpServerService>()
            .AddSystemService<IVersionService, VersionService>(1)
            .AddSystemService<IConfigService, ConfigService>()
            ;


        return services;
    }
}
