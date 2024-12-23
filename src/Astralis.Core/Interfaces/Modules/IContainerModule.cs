using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Astralis.Core.Interfaces.Modules;

public interface IContainerModule
{
    IServiceCollection RegisterServices(ILogger logger, IServiceCollection services);
}
