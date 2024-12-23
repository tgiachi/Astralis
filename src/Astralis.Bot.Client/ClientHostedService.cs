using System.Collections.Concurrent;
using System.Numerics;
using System.Reactive.Linq;
using Astralis.Core.Numerics;
using Astralis.Core.World.Chunks;
using Astralis.Network.Client.Interfaces;
using Astralis.Network.Packets;
using Astralis.Network.Serialization.Numerics;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Astralis.Bot.Client;

public class ClientHostedService : IHostedService
{
    private readonly INetworkClient _networkClient;
    private readonly ILogger _logger = Log.ForContext<ClientHostedService>();
    private readonly IDisposable _poolTimer;
    private IDisposable _movementTimer;

    private readonly ConcurrentDictionary<Vector3Int, ChunkEntity> _chunks = new();

    private Vector3 _playerPosition = Vector3.Zero;

    public ClientHostedService(INetworkClient networkClient)
    {
        _networkClient = networkClient;
        _networkClient.MessageReceived += (sender, args) =>
        {
            _logger.Information("Received message: {MessageType}: {Message}", args.GetType().Name, args);
        };

        _networkClient.SubscribeToMessage<PlayerStartPositionResponseMessage>().Subscribe(OnPlayerPositionReceived);

        _networkClient.SubscribeToMessage<WorldChunkResponseMessage>().Subscribe(OnChunkResponse);

        _networkClient.Connected += (sender, args) =>
        {
            _logger.Information("Connected to server.");

            _networkClient.SendMessageAsync(new MotdRequestMessage());
        };

        _poolTimer = Observable.Interval(TimeSpan.FromMilliseconds(30)).Subscribe(_ => { _networkClient.PoolEvents(); });
    }

    private void OnChunkResponse(WorldChunkResponseMessage obj)
    {
        var chunks = obj.Chunks;
        _logger.Information("Received chunk: {Chunk}", chunks);

        foreach (var chunk in chunks)
        {
            _chunks.AddOrUpdate(chunk.Position.ToVector3Int(), chunk.ToChunkEntity(), (key, value) => chunk.ToChunkEntity());
        }
    }

    private void OnPlayerPositionReceived(PlayerStartPositionResponseMessage obj)
    {
        _playerPosition = obj.Position.ToVector3();

        foreach (var chunkPosition in ChunkEntity.GetChunkPositionsAroundPosition(_playerPosition, 4))
        {
            _networkClient.SendMessageAsync(new WorldChunkRequestMessage(new SerializableVector3Int(chunkPosition)));
        }

        StartMovement();
    }

    private void StartMovement()
    {
        _movementTimer = Observable.Interval(TimeSpan.FromMilliseconds(300))
            .Subscribe(
                _ =>
                {
                    _playerPosition += new Vector3(0.1f, 0, 0);
                    _logger.Information("Sending player move request: {Position}", _playerPosition);
                    _networkClient.SendMessageAsync(new PlayerMoveRequestMessage(_playerPosition, Vector3.Zero));
                }
            );
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Starting client...");
        _networkClient.Connect("127.0.0.1", 5006);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _poolTimer.Dispose();
        _movementTimer.Dispose();
        return Task.CompletedTask;
    }
}
