namespace Astralis.Network.Interfaces.Metrics.Server;

public class NetworkServerStatsSnapshot
{
    public int ActiveConnections { get; set; }
    public int PeakConnections { get; set; }
    public long TotalMessagesSent { get; set; }
    public long TotalMessagesReceived { get; set; }
    public long TotalBytesSent { get; set; }
    public long TotalBytesReceived { get; set; }
    public double UptimeSeconds { get; set; }
}
