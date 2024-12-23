using System.Collections.Generic;

namespace Astralis.Network.Interfaces.Metrics.Server;

public class NetworkMetricsSnapshot
{
    public  NetworkServerStatsSnapshot ServerStats { get; set; }
    public  Dictionary<string, LiteNetStats> PeerStats { get; set; }
    public  Dictionary<string, MessageTypeStatsSnapshot> MessageTypeStats { get; set; }
    public  NetworkEvent[] RecentEvents { get; set; }
}
