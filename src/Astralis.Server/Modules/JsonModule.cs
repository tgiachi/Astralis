using Astralis.Core.Extensions;
using Astralis.Core.Interfaces.Modules;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Astralis.Server.Modules;

public class JsonModule : IContainerModule
{
    public IServiceCollection RegisterServices(ILogger logger, IServiceCollection services)
    {
        return services
            .RegisterDefaultJsonOptions();
    }
}
