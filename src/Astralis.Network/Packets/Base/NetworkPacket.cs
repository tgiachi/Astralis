using Astralis.Network.Interfaces.Packets;
using Astralis.Network.Types;
using LiteNetLib.Utils;

namespace Astralis.Network.Packets.Base;

public class NetworkPacket : INetworkPacket
{
    public NetworkPacketType PacketType { get; set; }
    public byte[] Payload { get; set; }
    public NetworkMessageType MessageType { get; set; }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put((byte)PacketType);
        writer.Put(Payload);
        writer.Put((byte)MessageType);
    }

    public void Deserialize(NetDataReader reader)
    {
        PacketType = (NetworkPacketType)reader.GetByte();
        Payload = reader.GetBytesWithLength();
        MessageType = (NetworkMessageType)reader.GetByte();
    }

    public override string ToString() =>
        $"PacketType: {PacketType}, MessageType: {MessageType}, Payload: {Payload.Length} bytes";
}
