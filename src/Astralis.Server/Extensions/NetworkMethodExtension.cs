using Astralis.Core.Extensions;
using Astralis.Core.Server.Extensions;
using Astralis.Network.Data.Internal;
using Astralis.Network.Encoders;
using Astralis.Network.Interfaces.Encoders;
using Astralis.Network.Interfaces.Metrics.Server;
using Astralis.Network.Interfaces.Services;
using Astralis.Network.Services;
using Astralis.Network.Types;
using Astralis.Server.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Astralis.Server.Extensions;

public static class NetworkMethodExtension
{
    public static IServiceCollection RegisterNetworkServer(this IServiceCollection services)
    {
        services
            .AddSingleton<INetworkSessionService, NetworkSessionService>()
            .AddSingleton<IMessageChannelService, MessageChannelService>()
            .AddSingleton<IMessageTypesService, MessageTypesService>()
            .AddSingleton<INetworkMessageFactory, NetworkMessageFactory>()
            .AddSingleton<IMessageParserWriterService, MessageParserWriterService>()
            .AddSingleton<IMessageDispatcherService, MessageDispatcherService>()
            .AddSingleton<NetworkServerMetrics>()
            .AddSystemService<INetworkServer, NetworkServer>(100);

        return services;
    }


    public static IServiceCollection RegisterNetworkMessageTypes(
        this IServiceCollection services, List<MessageTypeObject> messageTypes
    )
    {
        messageTypes.ForEach(s => services.AddToRegisterTypedList(s));

        return services;
    }

    public static IServiceCollection RegisterProtobufEncoder(this IServiceCollection services)
    {
        return services
            .AddSingleton<INetworkMessageEncoder, ProtobufEncoder>();
    }

    public static IServiceCollection RegisterProtobufDecoder(this IServiceCollection services)
    {
        return services
            .AddSingleton<INetworkMessageDecoder, ProtobufDecoder>();
    }


    public static IServiceCollection RegisterNetworkMessage(
        this IServiceCollection services, Type TypeOfMessage, NetworkMessageType messageType
    )
    {
        return services.AddToRegisterTypedList(new MessageTypeObject(messageType, TypeOfMessage));
    }

    public static IServiceCollection RegisterNetworkMessage<TMessage>(
        this IServiceCollection services, NetworkMessageType messageType
    )
    {
        return services.AddToRegisterTypedList(new MessageTypeObject(messageType, typeof(TMessage)));
    }
}
