namespace Astralis.Server.Data.Configs;

public class ProcessQueueConfig
{
    public int MaxConcurrentProcesses { get; set; } = 6;


    public ProcessQueueConfig()
    {
    }

    public ProcessQueueConfig(int maxConcurrentProcesses)
    {
        MaxConcurrentProcesses = maxConcurrentProcesses;
    }
}
