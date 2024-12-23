using System;
using System.Threading.Channels;
using Astralis.Network.Data.Internal;

namespace Astralis.Network.Interfaces.Services;

public interface IMessageChannelService : IDisposable
{
    ChannelReader<SessionNetworkPacket> IncomingReaderChannel { get; }
    ChannelWriter<SessionNetworkPacket> IncomingWriterChannel { get; }
    ChannelReader<SessionNetworkMessage> OutgoingReaderChannel { get; }
    ChannelWriter<SessionNetworkMessage> OutgoingWriterChannel { get; }



}
