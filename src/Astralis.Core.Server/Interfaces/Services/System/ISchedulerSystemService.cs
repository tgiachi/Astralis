using Astralis.Core.Interfaces.Services.Base;
using Astralis.Core.Server.Data.Metrics;
using Astralis.Core.Server.Interfaces.Scheduler;

namespace Astralis.Core.Server.Interfaces.Services.System;

public interface ISchedulerSystemService : IAstralisSystemService
{
    long CurrentTick { get; }
    void EnqueueAction(ISchedulerAction action);
    void EnqueueActions(IEnumerable<ISchedulerAction> actions);
    int ActionsInQueue { get; }
    void AddSchedulerJob(string name, TimeSpan totalSpan, Func<Task> action);
    IObservable<SchedulerMetrics> GetMetricsObservable();
}
