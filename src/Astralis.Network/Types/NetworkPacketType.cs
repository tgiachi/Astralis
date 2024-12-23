using System;

namespace Astralis.Network.Types;

[Flags]
public enum NetworkPacketType : byte
{
    None,
    Compressed
}
