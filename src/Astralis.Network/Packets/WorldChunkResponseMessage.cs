using System.Collections.Generic;
using Astralis.Core.World.Chunks;
using Astralis.Network.Interfaces.Messages;
using Astralis.Network.Serialization.Chunk;
using ProtoBuf;

namespace Astralis.Network.Packets;

[ProtoContract]
public class WorldChunkResponseMessage : INetworkMessage
{
    [ProtoMember(1)] public List<SerializableChunk> Chunks { get; set; }

    public WorldChunkResponseMessage()
    {
    }

    public WorldChunkResponseMessage(List<ChunkEntity> chunk)
    {
        Chunks = new List<SerializableChunk>();
        foreach (var c in chunk)
        {
            Chunks.Add(new SerializableChunk(c));
        }
    }

    public WorldChunkResponseMessage(params ChunkEntity[] chunk)
    {
        Chunks = new List<SerializableChunk>();
        foreach (var c in chunk)
        {
            Chunks.Add(new SerializableChunk(c));
        }
    }

    public override string ToString()
    {
        return $"WorldChunkResponseMessage: {Chunks.Count} chunks";
    }
}
