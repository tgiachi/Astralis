namespace Astralis.Core.Server.Data.Internal;

public class SchedulerJobData
{
    public string Name { get; set; }
    public Func<Task> Action { get; set; }
    public double TotalMs { get; set; }
    public double CurrentMs { get; set; }
}
