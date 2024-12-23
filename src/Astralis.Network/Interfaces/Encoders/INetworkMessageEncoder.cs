using Astralis.Network.Interfaces.Packets;
using Astralis.Network.Types;

namespace Astralis.Network.Interfaces.Encoders;

public interface INetworkMessageEncoder
{
    INetworkPacket Encode<TMessage>(TMessage message, NetworkMessageType messageType) where TMessage : class;
}
