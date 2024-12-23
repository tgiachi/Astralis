using Astralis.Core.Interfaces.Events;
using Astralis.Core.Server.Interfaces.Scheduler;

namespace Astralis.Core.Server.Events.Scheduler;

public record EnqueueSchedulerActionEvent(ISchedulerAction SchedulerAction) : IAstralisEvent;
