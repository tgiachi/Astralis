using System.Threading.Tasks;
using Astralis.Network.Data.Internal;
using Astralis.Network.Interfaces.Metrics.Server;
using Astralis.Network.Interfaces.Metrics.Types;
using Astralis.Network.Interfaces.Services;
using Astralis.Network.Packets.Base;
using Humanizer;
using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;

namespace Astralis.Network.Services;

public class MessageParserWriterService : IMessageParserWriterService
{
    private readonly ILogger _logger = Log.ForContext<MessageParserWriterService>();

    private readonly NetPacketProcessor _netPacketProcessor = new();


    private readonly NetworkServerMetrics _metrics;

    private readonly IMessageChannelService _messageChannelService;

    public MessageParserWriterService(
        IMessageChannelService messageChannelService, NetworkServerMetrics metrics
    )
    {
        _messageChannelService = messageChannelService;
        _metrics = metrics;
        _netPacketProcessor.SubscribeReusable<NetworkPacket, (NetPeer, int)>(OnReceivePacket);
    }

    private async void OnReceivePacket(NetworkPacket packet, (NetPeer peer, int bytes) data)
    {
        _logger.Debug(
            "Received packet from {peerId} ({Size}) type: {Type}",
            data.peer.Id,
            data.bytes.Bytes(),
            packet.MessageType
        );

        _metrics.TrackMessage(data.peer, packet.MessageType, MessageDirection.Incoming, data.bytes);
        _messageChannelService.IncomingWriterChannel.TryWrite(new SessionNetworkPacket(data.peer.Id.ToString(), packet));
    }

    public void ReadPackets(NetDataReader reader, NetPeer peer)
    {
        _netPacketProcessor.ReadAllPackets(reader, (peer, reader.AvailableBytes));
    }

    public async Task WriteMessageAsync(NetPeer peer, NetDataWriter writer, NetworkPacket message)
    {
        writer.Reset();

        _netPacketProcessor.Write(writer, message);

        _logger.Debug(">> Sending {Type}  ({Bytes}) to {peerId}", message.MessageType, writer.Length.Bytes(), peer.Id);

        peer.Send(writer, DeliveryMethod.ReliableOrdered);
    }
}
