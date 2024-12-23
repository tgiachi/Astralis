using Astralis.Core.World.Chunks;

namespace Astralis.Server.Services.Data.Internal;

public struct ChunkData
{
    public DateTime LastAccessed { get; set; }

    public ChunkEntity Chunk { get; set; }


    public ChunkData(ChunkEntity chunk)
    {
        LastAccessed = DateTime.Now;
        Chunk = chunk;
    }
}
