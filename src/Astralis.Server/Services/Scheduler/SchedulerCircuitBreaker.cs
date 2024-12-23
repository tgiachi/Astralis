using Astralis.Core.Server.Types;
using Serilog;

namespace Astralis.Server.Services.Scheduler;

public class SchedulerCircuitBreaker
{
    private readonly ILogger _logger = Log.Logger.ForContext<SchedulerCircuitBreaker>();
    private int _failureCount;
    private DateTime _lastFailure;
    private CircuitState _state = CircuitState.Closed;
    private readonly int _failureThreshold;
    private readonly TimeSpan _resetTimeout;

    public SchedulerCircuitBreaker(int failureThreshold = 5, int resetTimeoutSeconds = 30)
    {
        _failureThreshold = failureThreshold;
        _resetTimeout = TimeSpan.FromSeconds(resetTimeoutSeconds);
    }

    public bool CanExecute()
    {
        if (_state == CircuitState.Open)
        {
            if (DateTime.UtcNow - _lastFailure > _resetTimeout)
            {
                _state = CircuitState.HalfOpen;
                _logger.Information("Circuit breaker state changed to half-open");
                return true;
            }

            return false;
        }

        return true;
    }

    public void RecordSuccess()
    {
        if (_state == CircuitState.HalfOpen)
        {
            _state = CircuitState.Closed;
            _failureCount = 0;
            _logger.Information("Circuit breaker state changed to closed");
        }
    }

    public void RecordFailure()
    {
        _lastFailure = DateTime.UtcNow;
        _failureCount++;

        if (_failureCount >= _failureThreshold && _state == CircuitState.Closed)
        {
            _state = CircuitState.Open;
            _logger.Warning("Circuit breaker opened due to {FailureCount} failures", _failureCount);
        }
    }
}
