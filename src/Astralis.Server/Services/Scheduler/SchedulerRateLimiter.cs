namespace Astralis.Server.Services.Scheduler;

public class SchedulerRateLimiter
{
    private readonly SemaphoreSlim _semaphore;
    private readonly Queue<DateTime> _requestTimestamps = new();
    private readonly int _maxRequests;
    private readonly TimeSpan _interval;
    private readonly Lock _lock = new();

    public SchedulerRateLimiter(int maxRequests, TimeSpan interval)
    {
        _maxRequests = maxRequests;
        _interval = interval;
        _semaphore = new SemaphoreSlim(maxRequests, maxRequests);
    }

    public async Task<bool> TryAcquireAsync()
    {
        if (!await _semaphore.WaitAsync(TimeSpan.Zero))
        {
            return false;
        }

        try
        {
            lock (_lock)
            {
                var now = DateTime.UtcNow;
                while (_requestTimestamps.Count > 0 && now - _requestTimestamps.Peek() > _interval)
                {
                    _requestTimestamps.Dequeue();
                }

                if (_requestTimestamps.Count >= _maxRequests)
                {
                    return false;
                }

                _requestTimestamps.Enqueue(now);
                return true;
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
