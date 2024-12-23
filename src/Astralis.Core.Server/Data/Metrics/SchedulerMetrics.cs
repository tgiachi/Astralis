using System.Collections.Concurrent;

namespace Astralis.Core.Server.Data.Metrics;

public class SchedulerMetrics
{
    public int TotalActionsProcessed { get; set; }
    public int FailedActions { get; set; }
    public double AverageProcessingTimeMs { get; set; }
    public int CurrentQueueSize { get; set; }
    public int PeakQueueSize { get; set; }
    public ConcurrentDictionary<string, int> ActionTypeDistribution { get; set; } = new();

    public ConcurrentDictionary<int, int> PriorityDistribution { get; set; } = new();
    public double LastTickDurationMs { get; set; }
    public int CurrentMaxActionsPerTick { get; set; }
    public DateTime LastUpdated { get; set; }
}
