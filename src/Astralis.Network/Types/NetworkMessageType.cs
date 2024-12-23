namespace Astralis.Network.Types;

public enum NetworkMessageType : byte
{
    VersionRequest,
    VersionResponse,


    PingRequest,
    PongResponse,

    MotdRequest,
    MotdResponse,

    WorldChunkRequest,
    WorldChunkResponse,

    PlayerMoveRequest,
    PlayerMoveResponse,

    PlayerStartPositionRequest,
    PlayerStartPositionResponse,
}
