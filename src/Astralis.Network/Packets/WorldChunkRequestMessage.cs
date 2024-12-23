using Astralis.Network.Interfaces.Messages;
using Astralis.Network.Serialization.Numerics;
using ProtoBuf;

namespace Astralis.Network.Packets;

[ProtoContract]
public class WorldChunkRequestMessage : INetworkMessage
{
    [ProtoMember(1)]
    public SerializableVector3Int ChunkPosition { get; set; }

    public WorldChunkRequestMessage()
    {
    }

    public WorldChunkRequestMessage(SerializableVector3Int chunkPosition)
    {
        ChunkPosition = chunkPosition;
    }

    public override string ToString()
    {
        return $"WorldChunkRequestMessage: {ChunkPosition}";
    }

}
