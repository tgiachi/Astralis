using System.Collections.Generic;
using Astralis.Core.Numerics;
using Astralis.Core.World.Blocks;
using Astralis.Core.World.Chunks;
using Astralis.Network.Serialization.Numerics;
using ProtoBuf;

namespace Astralis.Network.Serialization.Chunk;

[ProtoContract]
public class SerializableChunk
{
    [ProtoMember(1)] public SerializableVector3Int Position { get; set; }

    [ProtoMember(2)] public List<BlockType> Blocks { get; set; }

    public SerializableChunk()
    {
        Position = new SerializableVector3Int();
        Blocks = new List<BlockType>();
    }

    public SerializableChunk(Vector3Int position, BlockType[] blocks)
    {
        Position = new SerializableVector3Int(position);
        Blocks = new List<BlockType>(blocks);
    }

    public SerializableChunk(ChunkEntity chunkEntity)
    {
        Position = new SerializableVector3Int(chunkEntity.Position);
        Blocks = new List<BlockType>(chunkEntity.Blocks);
    }

    public ChunkEntity ToChunkEntity()
    {
        return new ChunkEntity(Position.ToVector3Int(), Blocks.ToArray());
    }

    public override string ToString()
    {
        return $"SerializableChunk: {Position}";
    }
}
