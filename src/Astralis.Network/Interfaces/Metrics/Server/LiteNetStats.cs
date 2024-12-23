using System;
using LiteNetLib;

namespace Astralis.Network.Interfaces.Metrics.Server;

public class LiteNetStats
{
    public long PacketsSent { get; set; }
    public long PacketsReceived { get; set; }
    public long BytesSent { get; set; }
    public long BytesReceived { get; set; }
    public long PacketLoss { get; set; }
    public TimeSpan RoundTripTime { get; set; }
    public DateTime LastUpdated { get; private set; } = DateTime.UtcNow;

    public void Update(NetStatistics stats, int ping)
    {
        PacketsSent = stats.PacketsSent;
        PacketsReceived = stats.PacketsReceived;
        BytesSent = stats.BytesSent;
        BytesReceived = stats.BytesReceived;
        PacketLoss = stats.PacketLoss;
        RoundTripTime = TimeSpan.FromMilliseconds(ping);
        LastUpdated = DateTime.UtcNow;
    }
}
