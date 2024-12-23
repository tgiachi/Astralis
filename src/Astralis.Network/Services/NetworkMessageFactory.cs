using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Astralis.Core.Utils;
using Astralis.Network.Interfaces.Encoders;
using Astralis.Network.Interfaces.Messages;
using Astralis.Network.Interfaces.Packets;
using Astralis.Network.Interfaces.Services;
using Serilog;

namespace Astralis.Network.Services;

public class NetworkMessageFactory : INetworkMessageFactory
{
    private readonly ILogger _logger = Log.ForContext<NetworkMessageFactory>();

    private readonly IMessageTypesService _messageTypesService;

    private readonly INetworkMessageDecoder _decoder;

    private readonly INetworkMessageEncoder _encoder;

    public NetworkMessageFactory(
        IMessageTypesService messageTypesService, INetworkMessageDecoder decoder, INetworkMessageEncoder encoder
    )
    {
        _messageTypesService = messageTypesService;
        _decoder = decoder;
        _encoder = encoder;
    }


    public async Task<INetworkPacket> SerializeAsync<T>(T message) where T : class
    {
        if (_encoder == null)
        {
            _logger.Error("No encoder registered");
            throw new InvalidOperationException("No message encoder registered");
        }

        var startTime = Stopwatch.GetTimestamp();


        var encodedNetworkPacket = _encoder.Encode(message, _messageTypesService.GetMessageType(message.GetType()));

        var endTime = Stopwatch.GetTimestamp();

        _logger.Verbose(
            "Encoding message of type {messageType} took {time}ms",
            message.GetType().Name,
            StopwatchUtils.GetElapsedMilliseconds(startTime, endTime)
        );

        return encodedNetworkPacket;
    }

    public async Task<INetworkMessage> ParseAsync(INetworkPacket packet)
    {
        if (_decoder == null)
        {
            _logger.Error("No decoder registered");
            throw new InvalidOperationException("No message decoder registered");
        }

        var startTime = Stopwatch.GetTimestamp();

        var message = _decoder.Decode(packet, _messageTypesService.GetMessageType(packet.MessageType));

        var endTime = Stopwatch.GetTimestamp();

        _logger.Verbose(
            "Decoding message of type {messageType} took {time}ms",
            message.GetType().Name,
            StopwatchUtils.GetElapsedMilliseconds(startTime, endTime)
        );

        return message;
    }
}
