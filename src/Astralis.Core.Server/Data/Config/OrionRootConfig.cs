namespace Astralis.Core.Server.Data.Config;

public class OrionRootConfig
{
    public SchedulerServiceConfig Scheduler { get; set; } = new SchedulerServiceConfig(100, 50, 10);


}
