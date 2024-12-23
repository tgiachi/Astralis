using Astralis.Core.Server.Data.Internal;
using Astralis.Core.Server.Interfaces.Scheduler;
using Astralis.Core.Server.Types;

namespace Astralis.Core.Server.Data.Scheduler;

public class ScheduledSchedulerAction : ISchedulerAction
{
    public SchedulerPriorityType Priority { get; set; } = SchedulerPriorityType.Normal;

    private readonly Func<Task> _action;

    public ScheduledSchedulerAction(Func<Task> action)
    {
        _action = action;
    }


    public async Task<SchedulerActionResult> ExecuteAsync(double elapsedMilliseconds)
    {
        await _action();
        return SchedulerActionResult.Success;
    }
}
