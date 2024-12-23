using Astralis.Core.Interfaces.Services.Base;
using Astralis.Core.Server.Data.Metrics;

namespace Astralis.Core.Server.Interfaces.Services.System;

public interface IProcessQueueService : IAstralisSystemService, IDisposable
{
    Task<T> Enqueue<T>(string context, Func<T> func, CancellationToken cancellationToken = default);
    Task<T> Enqueue<T>(string context, Func<Task<T>> func, CancellationToken cancellationToken = default);
    Task Enqueue(string context, Action action, CancellationToken cancellationToken = default);
    Task Enqueue(string context, Func<Task> func, CancellationToken cancellationToken = default);
    IObservable<ProcessQueueMetric> GetMetrics();
}
