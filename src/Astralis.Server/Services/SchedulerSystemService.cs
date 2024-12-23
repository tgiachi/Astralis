using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Astralis.Core.Interfaces.EventBus;
using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Data.Config;
using Astralis.Core.Server.Data.Internal;
using Astralis.Core.Server.Data.Metrics;
using Astralis.Core.Server.Data.Scheduler;
using Astralis.Core.Server.Events.Scheduler;
using Astralis.Core.Server.Interfaces.Scheduler;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Core.Server.Types;
using Astralis.Server.Services.Scheduler;
using Serilog;

namespace Astralis.Server.Services;

public class SchedulerSystemService
    : ISchedulerSystemService, IEventBusListener<EnqueueSchedulerActionEvent>, IEventBusListener<AddSchedulerJobEvent>
{
    private const int MAX_QUEUE_SIZE = 10000;
    public const double MAX_PROCESSING_TIME = 100;
    public const int MAX_FAILED_ACTIONS = 1000;

    private readonly ILogger _logger = Log.Logger.ForContext<SchedulerSystemService>();
    private readonly PriorityQueue<ISchedulerAction, int> _actionQueue = new();
    private readonly SchedulerCircuitBreaker _circuitBreaker = new();
    private readonly SchedulerRateLimiter _rateLimiter;
    private readonly SchedulerMetrics _metrics = new();
    private readonly Subject<SchedulerMetrics> _metricsObservable = new();

    private readonly DateTime _startTime;

    private readonly List<SchedulerJobData> _schedulerJobs = [];

    private double _lastElapsedMs;


    public int ActionsInQueue => _actionQueue.Count;
    public long CurrentTick { get; private set; }

    private int _currentMaxActionsPerTick;


    private readonly SchedulerServiceConfig _config;

    private readonly ParallelOptions _parallelOptions;

    private IDisposable _tickSubscription;

    private IDisposable _schedulerJobSubscription;


    private const int _schedulerJobInterval = 100;


    public SchedulerSystemService(SchedulerServiceConfig config, IEventBusService eventBusService)
    {
        _config = config;
        _startTime = DateTime.UtcNow;
        eventBusService.Subscribe<EnqueueSchedulerActionEvent>(this);
        eventBusService.Subscribe<AddSchedulerJobEvent>(this);

        _rateLimiter = new SchedulerRateLimiter(1000, TimeSpan.FromSeconds(1));
        _currentMaxActionsPerTick = _config.InitialMaxActionPerTick;


        _metricsObservable.Sample(TimeSpan.FromSeconds(10))
            .Subscribe(metrics => LogDetailedMetrics());

        _logger.Debug("Creating scheduler service with config {config}", _config);
        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = _config.NumThreads - 1
        };
    }

    private async Task OnTickAsync()
    {
        if (!_circuitBreaker.CanExecute())
        {
            _logger.Warning("Circuit breaker is open, skipping tick");
            return;
        }

        if (!await _rateLimiter.TryAcquireAsync())
        {
            _logger.Warning("Rate limit exceeded, throttling");
            return;
        }

        var tickStopwatch = Stopwatch.StartNew();

        try
        {
            var startTime = Stopwatch.GetTimestamp();
            var actionsBatch = new List<ISchedulerAction>();

            var processedActions = 0;

            while (processedActions < _currentMaxActionsPerTick)
            {
                if (_actionQueue.Count == 0)
                {
                    break;
                }

                ISchedulerAction action = _actionQueue.Dequeue();

                if (action == null)
                {
                    continue;
                }

                actionsBatch.Add(action);
                processedActions++;
            }

            var sortedActions = actionsBatch;
            var successfullyProcessedActions = 0;

            await Parallel.ForEachAsync(
                sortedActions,
                _parallelOptions,
                async (action, _) =>
                {
                    try
                    {
                        if (action == null)
                        {
                            return;
                        }

                        var actionStopwatch = Stopwatch.StartNew();


                        var result = await action.ExecuteAsync(_lastElapsedMs);

                        actionStopwatch.Stop();
                        var processingTimeMs = actionStopwatch.Elapsed.TotalMilliseconds;
                        UpdateMetrics(action, processingTimeMs);


                        if (result.ResultType == SchedulerActionResultType.Progress ||
                            result.ResultType == SchedulerActionResultType.Replace)
                        {
                            _actionQueue.Enqueue(action, (int)action.Priority);
                        }


                        Interlocked.Increment(ref successfullyProcessedActions);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Error processing action {action}, re queue ", action);
                        // requeue action
                        _actionQueue.Enqueue(action, (int)action.Priority);
                    }
                }
            );

            var remainingActionsCount = 0;
            if (successfullyProcessedActions < actionsBatch.Count)
            {
                var remainingActions = sortedActions.Skip(successfullyProcessedActions).ToList();
                foreach (var action in remainingActions)
                {
                    _logger.Verbose(
                        "Decreasing priority of action {action} from: {InitialPriority} -> {Destination}",
                        action,
                        action.Priority,
                        action.Priority - 1
                    );
                    action.Priority -= 1;
                    _actionQueue.Enqueue(action, (int)action.Priority);
                }

                remainingActionsCount = remainingActions.Count;
            }

            var endTime = Stopwatch.GetTimestamp();


            _lastElapsedMs = (endTime - startTime) * 1000 / (double)Stopwatch.Frequency;


            var oldMaxActionsPerTick = _currentMaxActionsPerTick;


            _currentMaxActionsPerTick = successfullyProcessedActions < _currentMaxActionsPerTick
                ? Math.Max(_config.InitialMaxActionPerTick / 2, 1)
                : _currentMaxActionsPerTick + 10;


            if (oldMaxActionsPerTick != _currentMaxActionsPerTick)
            {
                _logger.Debug(
                    "Max actions per tick changed from {oldMaxActionsPerTick} to {newMaxActionsPerTick} on tick {Tick} (queue: {Queue})",
                    oldMaxActionsPerTick,
                    _currentMaxActionsPerTick,
                    CurrentTick,
                    _actionQueue.Count
                );
            }

            _metrics.TotalActionsProcessed += successfullyProcessedActions;
            _metrics.FailedActions += (actionsBatch.Count - successfullyProcessedActions);
            _metrics.CurrentQueueSize = _actionQueue.Count;
            _metrics.PeakQueueSize = Math.Max(_metrics.PeakQueueSize, _actionQueue.Count);
            _metrics.LastTickDurationMs = _lastElapsedMs;
            _metrics.CurrentMaxActionsPerTick = _currentMaxActionsPerTick;
            _metrics.LastUpdated = DateTime.UtcNow;

            _circuitBreaker.RecordSuccess();
        }
        catch
        {
            _circuitBreaker.RecordFailure();
            throw;
        }
        finally
        {
            tickStopwatch.Stop();

            _metrics.LastTickDurationMs = tickStopwatch.ElapsedMilliseconds;

            _metricsObservable.OnNext(_metrics);

            if (CurrentTick + 1 >= 1_000_000)
            {
                CurrentTick = 0;
            }

            CurrentTick++;
        }
    }

    private void UpdateMetrics(ISchedulerAction action, double processingTimeMs)
    {
        var actionType = action.GetType().Name;
        var priority = (int)action.Priority;


        _metrics.ActionTypeDistribution.AddOrUpdate(
            actionType,
            1,
            (_, count) => count + 1
        );


        _metrics.PriorityDistribution.AddOrUpdate(
            priority,
            1,
            (_, count) => count + 1
        );


        const double alpha = 0.1;
        _metrics.AverageProcessingTimeMs = (_metrics.AverageProcessingTimeMs * (1 - alpha)) + (processingTimeMs * alpha);
    }


    public void EnqueueAction(ISchedulerAction action)
    {
        _actionQueue.Enqueue(action, (int)action.Priority);
    }

    public void EnqueueActions(IEnumerable<ISchedulerAction> actions)
    {
        foreach (var action in actions)
        {
            _actionQueue.Enqueue(action, (int)action.Priority);
        }
    }

    private bool DetermineHealthStatus(out string reason)
    {
        if (_metrics.CurrentQueueSize > MAX_QUEUE_SIZE)
        {
            reason = $"Queue overflow ({_metrics.CurrentQueueSize})";
            return false;
        }

        if (_metrics.AverageProcessingTimeMs > MAX_PROCESSING_TIME)
        {
            reason = $"High latency ({_metrics.AverageProcessingTimeMs:F1}ms)";
            return false;
        }

        if (_metrics.FailedActions > MAX_FAILED_ACTIONS)
        {
            reason = $"Too many failures ({_metrics.FailedActions})";
            return false;
        }

        reason = "OK";
        return true;
    }

    private void LogDetailedMetrics()
    {
        var isHealthy = DetermineHealthStatus(out string healthReason);
        var healthStatus = isHealthy ? "OK" : "WARNING";

        var uptime = DateTime.UtcNow - _startTime;
        var actionRate = _metrics.TotalActionsProcessed / uptime.TotalSeconds;

        _logger.Information(
            "[Scheduler] Status: {HealthStatus} | Tick: {Tick} " +
            "Actions/s: {ActionRate:F1} | " +
            "Avg Process: {AvgTime:F1}ms | " +
            "Queue: {QueueSize}/{PeakQueue} | " +
            "Failed: {FailedActions} | " +
            "Health: {HealthReason}",
            healthStatus,
            CurrentTick,
            actionRate,
            _metrics.AverageProcessingTimeMs,
            _metrics.CurrentQueueSize,
            _metrics.PeakQueueSize,
            _metrics.FailedActions,
            healthReason
        );
    }


    public Task StartAsync()
    {
        _logger.Debug(
            "Starting scheduler service with initial actions per tick: {maxActionsPerTick}, interval: {Interval} and tasks: {NumTasks} ",
            _config.InitialMaxActionPerTick,
            _config.TickInterval,
            _config.NumThreads
        );
        _tickSubscription = Observable.Interval(TimeSpan.FromMilliseconds(_config.TickInterval))
            .Throttle(_ => Observable.FromAsync(OnTickAsync))
            .Subscribe();


        _schedulerJobSubscription = Observable.Interval(TimeSpan.FromMilliseconds(_schedulerJobInterval))
            .Subscribe(_ => OnSchedulerJob());

        return Task.CompletedTask;
    }

    public void AddSchedulerJob(string name, TimeSpan totalMs, Func<Task> action)
    {
        _logger.Debug("Adding scheduler job {name} with total ms {totalMs}", name, totalMs.TotalMilliseconds);
        _schedulerJobs.Add(
            new SchedulerJobData
            {
                Name = name,
                TotalMs = totalMs.TotalMilliseconds,
                Action = action
            }
        );
    }

    public IObservable<SchedulerMetrics> GetMetricsObservable()
    {
        return _metricsObservable;
    }

    private void OnSchedulerJob()
    {
        foreach (var job in _schedulerJobs)
        {
            job.CurrentMs += _schedulerJobInterval;

            if (job.CurrentMs >= job.TotalMs)
            {
                job.CurrentMs = 0;
                _logger.Verbose("Executing scheduler job {job}", job.Name);
                EnqueueAction(new ScheduledSchedulerAction(job.Action));
            }
        }
    }

    public Task StopAsync()
    {
        _tickSubscription.Dispose();
        _schedulerJobSubscription.Dispose();

        return Task.CompletedTask;
    }

    public Task OnEventAsync(EnqueueSchedulerActionEvent message)
    {
        EnqueueAction(message.SchedulerAction);

        return Task.CompletedTask;
    }

    public Task OnEventAsync(AddSchedulerJobEvent message)
    {
        AddSchedulerJob(message.Name, message.TotalSpan, message.Action);

        return Task.CompletedTask;
    }
}
