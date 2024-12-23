using Astralis.Network.Types;
using LiteNetLib.Utils;

namespace Astralis.Network.Interfaces.Packets;

public interface INetworkPacket : INetSerializable
{
    NetworkPacketType PacketType { get; set; }
    byte[] Payload { get; set; }
    NetworkMessageType MessageType { get; set; }
}
