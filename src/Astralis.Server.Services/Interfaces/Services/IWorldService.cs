using System.Numerics;
using Astralis.Core.Interfaces.Services.Base;
using Astralis.Core.Numerics;
using Astralis.Core.World.Chunks;

namespace Astralis.Server.Services.Interfaces.Services;

public interface IWorldService : IAstralisGameService
{
    Task SaveWorldAsync(CancellationToken cancellationToken = default);
    Task<ChunkEntity> GetChunkByWorldPositionAsync(Vector3 position, CancellationToken cancellationToken = default);
    Task<List<ChunkEntity>> GetChunksByWorldPositionAsync(Vector3 position, int radius, CancellationToken cancellationToken = default);
    Task<ChunkEntity> GetChunkByChunkPositionAsync(Vector3Int position, CancellationToken cancellationToken = default);
    Task<List<ChunkEntity>> GetChunksByChunkPositionAsync(Vector3Int position, int radius, CancellationToken cancellationToken = default);

    Task PreloadChunksAsync(Vector3 position, int radius, CancellationToken cancellationToken = default);
}
