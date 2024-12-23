using Astralis.Core.Extensions;
using Astralis.Core.Interfaces.Services;
using Astralis.Core.Numerics;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Core.Server.Services.Base;
using Astralis.Network.Data.Events.Clients;
using Astralis.Network.Interfaces.Services;
using Astralis.Network.Packets;
using Astralis.Server.Services.Interfaces.Services;
using Serilog;

namespace Astralis.Server.Services.Handlers;

public class PlayerHandler : BaseHandlerService
{
    private readonly IVersionService _versionService;
    private readonly IWorldService _worldService;

    private readonly ILogger _logger = Log.ForContext<PlayerHandler>();

    public PlayerHandler(
        IEventBusService eventBusService, INetworkServer networkServer, IVersionService versionService,
        IWorldService worldService
    ) : base(eventBusService, networkServer)
    {
        _versionService = versionService;
        _worldService = worldService;
        SubscribeEventAsync<ClientConnectedEvent>(OnClientConnected);
    }

    private async Task OnClientConnected(ClientConnectedEvent @event)
    {
        var randomPosition = new Vector3Int(0, 0, 0).RandomVector3Int(-100, 100);
        _logger.Information("Player {PlayerId} connected at {Position}", @event.SessionId, randomPosition);

        await _worldService.PreloadChunksAsync(randomPosition.ToVector3(), 6);
        await SendNetworkMessageAsync(@event.SessionId, new VersionResponseMessage(_versionService.GetVersion()));
        await SendNetworkMessageAsync(@event.SessionId, new PlayerStartPositionResponseMessage(randomPosition.ToVector3()));
    }
}
