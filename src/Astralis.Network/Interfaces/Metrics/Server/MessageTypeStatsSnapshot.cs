using System;

namespace Astralis.Network.Interfaces.Metrics.Server;

public class MessageTypeStatsSnapshot
{
    public long Sent { get; set; }
    public long Received { get; set; }
    public DateTime LastSent { get; set; }
    public DateTime LastReceived { get; set; }
}
