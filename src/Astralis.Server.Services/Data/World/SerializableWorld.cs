using Astralis.Network.Serialization.Chunk;
using ProtoBuf;

namespace Astralis.Server.Services.Data.World;

[ProtoContract]
public class SerializableWorld
{
    [ProtoMember(1)]
    public long Seed { get; set; }

    [ProtoMember(2)]
    public List<SerializableChunk> Chunks { get; set; }
}
