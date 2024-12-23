using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Interfaces.Services.Handlers;
using Astralis.Network.Interfaces.Messages;
using Astralis.Network.Interfaces.Services;
using Serilog;

namespace Astralis.Core.Server.Services.Base;

public abstract class BaseHandlerService : BaseGameService, IAstralisHandlerService
{
    protected ILogger Logger { get; }

    private readonly INetworkServer _networkServer;

    protected BaseHandlerService(IEventBusService eventBusService, INetworkServer networkServer) : base(
        eventBusService
    )
    {
        _networkServer = networkServer;
        Logger = Log.Logger.ForContext(GetType());
    }


    protected void RegisterNetworkListener<TMessage>(Func<string, TMessage, ValueTask> listener)
        where TMessage : class, INetworkMessage
    {
        _networkServer.RegisterMessageListener(listener);
    }

    protected void SubscribeNetworkEvent<TMessage>(Func<string, TMessage, ValueTask> listener)
        where TMessage : class, INetworkMessage
    {
        _networkServer.RegisterMessageListener(listener);
    }

    protected async Task SendNetworkMessageAsync<TMessage>(string sessionId, TMessage message)
        where TMessage : class, INetworkMessage
    {
        await _networkServer.SendMessageAsync(sessionId, message);
    }

    protected async Task SendNetworkBroadcastAsync<TMessage>(TMessage message)
        where TMessage : class, INetworkMessage
    {
        await _networkServer.BroadcastMessageAsync(message);
    }
}
