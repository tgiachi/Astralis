using System.Threading.Tasks;
using Astralis.Network.Packets.Base;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Astralis.Network.Interfaces.Services;

public interface IMessageParserWriterService
{
    void ReadPackets(NetDataReader reader, NetPeer peer);

    Task WriteMessageAsync(NetPeer peer, NetDataWriter writer, NetworkPacket message);
}
