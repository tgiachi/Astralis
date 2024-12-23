using Astralis.Core.Server.Interfaces.Scheduler;
using Astralis.Core.Server.Types;

namespace Astralis.Core.Server.Data.Internal;

public class SchedulerActionResult
{
    public SchedulerActionResultType ResultType { get; set; }

    public ISchedulerAction Action { get; set; }


    public static SchedulerActionResult Success => new SchedulerActionResult
    {
        ResultType = SchedulerActionResultType.Success
    };
}
