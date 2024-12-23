using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks.Dataflow;
using Astralis.Core.Server.Data.Metrics;
using Astralis.Core.Server.Extensions;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Server.Data.Configs;
using Serilog;

namespace Astralis.Server.Services;

public class ProcessQueueService : IProcessQueueService
{
    private readonly ILogger _logger = Log.ForContext<ProcessQueueService>();
    private readonly int _maxParallelTasks;
    private readonly ConcurrentDictionary<string, ActionBlock<Func<Task>>> _queues;
    private readonly ConcurrentDictionary<string, ProcessStats> _stats;
    private readonly Subject<ProcessQueueMetric> _metricsSubject;
    private readonly CancellationTokenSource _globalCts;
    private readonly IDisposable _metricsSubscription;

    public ProcessQueueService(ProcessQueueConfig processQueueConfig)
    {
        _maxParallelTasks = processQueueConfig.MaxConcurrentProcesses;
        _queues = new ConcurrentDictionary<string, ActionBlock<Func<Task>>>();
        _stats = new ConcurrentDictionary<string, ProcessStats>();
        _metricsSubject = new Subject<ProcessQueueMetric>();
        _globalCts = new CancellationTokenSource();

        _metricsSubscription = Observable
            .Interval(TimeSpan.FromSeconds(10))
            .Subscribe(_ => EmitMetrics());

        GetOrCreateQueue(ProcessQueueServiceExtension.DefaultContext);
        GetOrCreateQueue(ProcessQueueServiceExtension.WorldGenerationContext);
    }

    public Task Enqueue(string context, Action action, CancellationToken cancellationToken = default)
    {
        return Enqueue(
            context,
            () =>
            {
                action();
                return Task.CompletedTask;
            },
            cancellationToken
        );
    }

    // Overload per Func<T>
    public Task<T> Enqueue<T>(string context, Func<T> func, CancellationToken cancellationToken = default)
    {
        return Enqueue(context, () => Task.FromResult(func()), cancellationToken);
    }

    // Overload per Func<Task>
    public Task Enqueue(string context, Func<Task> func, CancellationToken cancellationToken = default)
    {
        var queue = GetOrCreateQueue(context);
        var stats = GetOrCreateStats(context);

        var tcs = new TaskCompletionSource();


        _logger.Debug("Enqueueing task for context {Context} current {Size}", context, stats.QueuedItems);


        queue.Post(
            async () =>
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _globalCts.Token);
                    if (cts.Token.IsCancellationRequested)
                    {
                        tcs.SetCanceled(cts.Token);
                        return;
                    }

                    await func();
                    stats.IncrementProcessed(sw.Elapsed);
                    tcs.SetResult();
                }
                catch (Exception ex)
                {
                    stats.IncrementFailed();
                    tcs.SetException(ex);
                }
            }
        );

        stats.IncrementQueued();
        return tcs.Task;
    }

    public Task<T> Enqueue<T>(string context, Func<Task<T>> func, CancellationToken cancellationToken = default)
    {
        var queue = GetOrCreateQueue(context);
        var stats = GetOrCreateStats(context);

        var tcs = new TaskCompletionSource<T>();

        _logger.Debug("Enqueueing task for context {Context} current {Size}", context, queue.InputCount);

        queue.Post(
            async () =>
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _globalCts.Token);
                    if (cts.Token.IsCancellationRequested)
                    {
                        tcs.SetCanceled(cancellationToken);
                        return;
                    }

                    var result = await func();
                    stats.IncrementProcessed(sw.Elapsed);
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    stats.IncrementFailed();
                    tcs.SetException(ex);
                }
            }
        );

        stats.IncrementQueued();
        return tcs.Task;
    }

    public IObservable<ProcessQueueMetric> GetMetrics() => _metricsSubject.AsObservable();

    private ActionBlock<Func<Task>> GetOrCreateQueue(string context)
    {
        context = context.ToLower();

        return _queues.GetOrAdd(
            context,
            _ => new ActionBlock<Func<Task>>(
                async task => await task(),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = _maxParallelTasks,
                    CancellationToken = _globalCts.Token
                }
            )
        );
    }

    private ProcessStats GetOrCreateStats(string context)
    {
        return _stats.GetOrAdd(context, _ => new ProcessStats());
    }

    private void EmitMetrics()
    {
        foreach (var (context, stats) in _stats)
        {
            var metric = new ProcessQueueMetric(
                context,
                stats.QueuedItems,
                stats.ProcessedItems,
                stats.FailedItems,
                stats.AverageProcessingTime
            );
            _metricsSubject.OnNext(metric);
        }
    }

    public void Dispose()
    {
        _metricsSubscription.Dispose();
        _metricsSubject.Dispose();
        _globalCts.Dispose();
    }

    public Task StartAsync()
    {
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }
}
