using Astralis.Core.Interfaces.Events;

namespace Astralis.Core.Server.Events.Scheduler;

public record AddSchedulerJobEvent(string Name, TimeSpan TotalSpan, Func<Task> Action) : IAstralisEvent;
