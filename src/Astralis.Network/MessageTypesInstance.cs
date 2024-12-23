using System;
using System.Collections.Generic;
using System.Linq;
using Astralis.Network.Data.Internal;
using Astralis.Network.Packets;
using Astralis.Network.Types;

namespace Astralis.Network;

public static class MessageTypesInstance
{
    public static readonly Dictionary<NetworkMessageType, Type> MessageTypes = new()
    {
        { NetworkMessageType.VersionRequest, typeof(VersionRequestMessage) },
        { NetworkMessageType.VersionResponse, typeof(VersionResponseMessage) },
        { NetworkMessageType.PingRequest, typeof(PingRequestMessage) },
        { NetworkMessageType.PongResponse, typeof(PongResponseMessage) },
        { NetworkMessageType.WorldChunkRequest, typeof(WorldChunkRequestMessage) },
        { NetworkMessageType.WorldChunkResponse, typeof(WorldChunkResponseMessage) },
        { NetworkMessageType.MotdRequest, typeof(MotdRequestMessage) },
        { NetworkMessageType.MotdResponse, typeof(MotdResponseMessage) },
        { NetworkMessageType.PlayerMoveRequest, typeof(PlayerMoveRequestMessage) },
        { NetworkMessageType.PlayerMoveResponse, typeof(PlayerMoveResponseMessage) },
        { NetworkMessageType.PlayerStartPositionRequest, typeof(PlayerStartPositionRequestMessage) },
        { NetworkMessageType.PlayerStartPositionResponse, typeof(PlayerStartPositionResponseMessage) },
    };


    public static readonly List<MessageTypeObject> MessageTypesList =
        MessageTypes.Select(x => new MessageTypeObject(x.Key, x.Value)).ToList();
}
