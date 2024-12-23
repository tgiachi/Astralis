using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Services.Base;
using Astralis.Network.Interfaces.Services;
using Astralis.Network.Packets;
using Astralis.Server.Services.Interfaces.Services;
using Serilog;

namespace Astralis.Server.Services.Handlers;

public class WorldHandler : BaseHandlerService
{
    private readonly IWorldService _worldService;
    private readonly ILogger _logger = Log.ForContext<WorldHandler>();

    public WorldHandler(IEventBusService eventBusService, INetworkServer networkServer, IWorldService worldService) : base(
        eventBusService,
        networkServer
    )
    {
        _worldService = worldService;

        SubscribeNetworkEvent<WorldChunkRequestMessage>(OnChunkRequested);
        SubscribeNetworkEvent<PlayerMoveRequestMessage>(OnPlayerMoveRequested);
    }


    private async ValueTask OnChunkRequested(string sessionId, WorldChunkRequestMessage request)
    {
        var chunk = await _worldService.GetChunkByChunkPositionAsync(request.ChunkPosition.ToVector3Int());
        await SendNetworkMessageAsync(sessionId, new WorldChunkResponseMessage(chunk));
    }

    private async ValueTask OnPlayerMoveRequested(string sessionId, PlayerMoveRequestMessage request)
    {
        _logger.Information("Player {PlayerId} moved to {Position}", sessionId, request.Position);
        // var chunk = await _worldService.GetChunksByWorldPositionAsync(request.Position.ToVector3(), 1);

        // await SendNetworkMessageAsync(sessionId, new WorldChunkResponseMessage(chunk));
    }
}
