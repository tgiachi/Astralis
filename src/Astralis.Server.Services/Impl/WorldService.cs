using System.Collections.Concurrent;
using System.Numerics;
using Astralis.Core.Interfaces.Services;
using Astralis.Core.Numerics;
using Astralis.Core.Server.Attributes.Scripts;
using Astralis.Core.Server.Data.Directories;
using Astralis.Core.Server.Extensions;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Core.Server.Services.Base;
using Astralis.Core.Server.Types;
using Astralis.Core.World.Blocks;
using Astralis.Core.World.Chunks;
using Astralis.Network.Serialization.Chunk;
using Astralis.Network.Serialization.Numerics;
using Astralis.Server.Services.Data.Configs;
using Astralis.Server.Services.Data.Internal;
using Astralis.Server.Services.Data.World;
using Astralis.Server.Services.Events.World;
using Astralis.Server.Services.Interfaces.Services;
using ProtoBuf;
using Serilog;

namespace Astralis.Server.Services.Impl;

public class WorldService : BaseGameService, IWorldService
{
    private readonly ILogger _logger = Log.ForContext<WorldService>();

    private readonly ConcurrentDictionary<Vector3Int, ChunkData> _chunks = new();

    private readonly IProcessQueueService _processQueueService;
    private readonly ISchedulerSystemService _schedulerSystemService;
    private readonly DirectoriesConfig _directoriesConfig;


    [ScriptConfigVariable("world_config")] public WorldConfig WorldConfig { get; set; }

    public WorldService(
        IEventBusService eventBusService, IProcessQueueService processQueueService, DirectoriesConfig directoriesConfig,
        ISchedulerSystemService schedulerSystemService
    ) : base(eventBusService)
    {
        _processQueueService = processQueueService;
        _directoriesConfig = directoriesConfig;
        _schedulerSystemService = schedulerSystemService;

        SubscribeEventAsync<WorldSaveEvent>(OnWorldSaveAsync);
    }

    private Task OnWorldSaveAsync(WorldSaveEvent arg)
    {
        return SaveWorldAsync();
    }

    public Task SaveWorldAsync(CancellationToken cancellationToken = default)
    {
        return _processQueueService.EnqueueDefault(
            () =>
            {
                _logger.Information("Saving world...");

                var worldData = new SerializableWorld
                {
                    Seed = WorldConfig.Seed,
                    Chunks = new List<SerializableChunk>()
                };

                worldData.Chunks.AddRange(
                    _chunks.Select(
                        x => new SerializableChunk
                        {
                            Position = new SerializableVector3Int(x.Key),
                            Blocks = [..x.Value.Chunk.Blocks]
                        }
                    )
                );


                var chunkFilePath = Path.Combine(
                    _directoriesConfig[DirectoryType.Worlds],
                    WorldConfig.Seed + ".world"
                );

                _logger.Debug("Saving world to {Path}", chunkFilePath);


                using var file = File.Create(chunkFilePath);
                Serializer.Serialize(file, worldData);


                return Task.CompletedTask;
            },
            cancellationToken
        );
    }

    public Task<ChunkEntity> GetChunkByWorldPositionAsync(Vector3 position, CancellationToken cancellationToken = default)
    {
        var chunkPosition = new Vector3Int(
            (int)position.X / ChunkEntity.CHUNK_SIZE,
            (int)position.Y / ChunkEntity.CHUNK_HEIGHT,
            (int)position.Z / ChunkEntity.CHUNK_SIZE
        );

        return GetOrAddChunkAsync(chunkPosition);
    }

    public async Task<List<ChunkEntity>> GetChunksByWorldPositionAsync(
        Vector3 position, int radius, CancellationToken cancellationToken = default
    )
    {
        var chunks = new List<ChunkEntity>();

        for (var x = -radius; x <= radius; x++)
        {
            for (var y = -radius; y <= radius; y++)
            {
                for (var z = -radius; z <= radius; z++)
                {
                    var chunkPosition = new Vector3Int(
                        (int)position.X / ChunkEntity.CHUNK_SIZE + x,
                        (int)position.Y / ChunkEntity.CHUNK_HEIGHT + y,
                        (int)position.Z / ChunkEntity.CHUNK_SIZE + z
                    );

                    chunks.Add(await GetOrAddChunkAsync(chunkPosition));
                }
            }
        }

        return chunks;
    }

    public Task<ChunkEntity> GetChunkByChunkPositionAsync(Vector3Int position, CancellationToken cancellationToken = default)
    {
        return GetOrAddChunkAsync(position);
    }

    public async Task<List<ChunkEntity>> GetChunksByChunkPositionAsync(Vector3Int position, int radius, CancellationToken cancellationToken = default)
    {
        var chunks = new List<ChunkEntity>();

        for (var x = -radius; x <= radius; x++)
        {
            for (var y = -radius; y <= radius; y++)
            {
                for (var z = -radius; z <= radius; z++)
                {
                    var chunkPosition = new Vector3Int(
                        position.X + x,
                        position.Y + y,
                        position.Z + z
                    );

                    chunks.Add(await GetOrAddChunkAsync(chunkPosition));
                }
            }
        }

        return chunks;
    }

    public Task PreloadChunksAsync(Vector3 position, int radius, CancellationToken cancellationToken = default)
    {
        _logger.Information("Preloading chunks around {Position} with radius {Radius}", position, radius);
        return _processQueueService.EnqueueDefault(
            () =>
            {
                for (var x = -radius; x <= radius; x++)
                {
                    for (var y = -radius; y <= radius; y++)
                    {
                        for (var z = -radius; z <= radius; z++)
                        {
                            var chunkPosition = new Vector3Int(
                                (int)position.X / ChunkEntity.CHUNK_SIZE + x,
                                (int)position.Y / ChunkEntity.CHUNK_HEIGHT + y,
                                (int)position.Z / ChunkEntity.CHUNK_SIZE + z
                            );

                            _ = GetOrAddChunkAsync(chunkPosition);
                        }
                    }
                }
            },
            cancellationToken
        );
    }

    private async Task<ChunkEntity> GetOrAddChunkAsync(Vector3Int position)
    {
        if (_chunks.TryGetValue(position, out var chunkData))
        {
            chunkData.LastAccessed = DateTime.Now;
            return chunkData.Chunk;
        }

        var chunk = await _processQueueService.Enqueue(
            ProcessQueueServiceExtension.WorldGenerationContext,
            () => GenerateChunkAsync(position)
        );
        chunkData = new ChunkData(chunk);

        _chunks.TryAdd(position, chunkData);

        return chunk;
    }

    private async Task<ChunkEntity> GenerateChunkAsync(Vector3Int position)
    {
        _logger.Information("Generating chunk at {Position}", position);


        var chunk = new ChunkEntity(position);

        for (int i = 0; i < chunk.Blocks.Length; i++)
        {
            chunk.Blocks[i] = Random.Shared.Next(0, 2) == 0 ? BlockType.Air : BlockType.Dirt;
        }

        _logger.Debug("Generated chunk at {Position}: {Stats}", position, chunk.GetBlockStats());

        return chunk;
    }

    public override async Task OnReadyAsync()
    {
        if (WorldConfig.Seed == -1)
        {
            WorldConfig.Seed = Random.Shared.NextInt64();
        }

        _logger.Information("World seed is {Seed}", WorldConfig.Seed);


        _schedulerSystemService.AddSchedulerJob("UnloadUnusedChunks", TimeSpan.FromMinutes(5), UnloadUnusedChunksAsync);


        _schedulerSystemService.AddSchedulerJob(
            "SaveWorld",
            TimeSpan.FromSeconds(WorldConfig.WorldSaveInterval),
            () => SaveWorldAsync(CancellationToken.None)
        );


        Task.Run(
            async () => { PreLoadChunksAsync(); }
        );
    }

    private async Task PreLoadChunksAsync()
    {
        for (var x = -WorldConfig.RenderDistance; x <= WorldConfig.RenderDistance; x++)
        {
            for (var z = -WorldConfig.RenderDistance; z <= WorldConfig.RenderDistance; z++)
            {
                var position = new Vector3(x * ChunkEntity.CHUNK_SIZE, 0, z * ChunkEntity.CHUNK_SIZE);
                _ = await GetChunkByWorldPositionAsync(position);
            }
        }
    }

    private Task UnloadUnusedChunksAsync()
    {
        return _processQueueService.EnqueueDefault(
            () =>
            {
                var now = DateTime.Now;

                foreach (var (position, chunkData) in _chunks)
                {
                    if ((now - chunkData.LastAccessed).TotalSeconds > WorldConfig.ChunkUnloadTime &&
                        !chunkData.Chunk.IsModified)
                    {
                        _chunks.TryRemove(position, out _);
                    }
                }

                return Task.CompletedTask;
            }
        );
    }
}
