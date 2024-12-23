using Astralis.Core.Interfaces.Modules;
using Astralis.Network;
using Astralis.Server.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Astralis.Server.Modules;

public class NetworkServiceModule : IContainerModule
{
    public IServiceCollection RegisterServices(
        ILogger logger, IServiceCollection services
    )
    {
        return services
            .RegisterNetworkServer()
            .RegisterProtobufEncoder()
            .RegisterProtobufDecoder()
            .RegisterNetworkMessageTypes(MessageTypesInstance.MessageTypesList);
    }
}
