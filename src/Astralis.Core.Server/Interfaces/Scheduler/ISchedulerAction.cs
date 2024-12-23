using Astralis.Core.Server.Data.Internal;
using Astralis.Core.Server.Types;

namespace Astralis.Core.Server.Interfaces.Scheduler;

public interface ISchedulerAction
{
    Task<SchedulerActionResult> ExecuteAsync(double elapsedMilliseconds);
    SchedulerPriorityType Priority { get; set; }
}
