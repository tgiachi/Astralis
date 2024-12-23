using System;
using Astralis.Network.Interfaces.Messages;
using Astralis.Network.Interfaces.Packets;

namespace Astralis.Network.Interfaces.Encoders;

public interface INetworkMessageDecoder
{
    INetworkMessage Decode(INetworkPacket packet, Type type);
}
