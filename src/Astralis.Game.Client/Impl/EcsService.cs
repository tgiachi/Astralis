using Arch.Core;
using Astralis.Core.Attributes.Services;
using Astralis.Game.Client.Interfaces.Services;
using Schedulers;
using Serilog;

namespace Astralis.Game.Client.Impl;

[OrionSystemService(1)]
public class EcsService : IEcsService
{
    private readonly ILogger _logger = Log.ForContext<EcsService>();
    private readonly World _world;

    public EcsService(World world)
    {
        _world = world;
        var config = new JobScheduler.Config
        {
            ThreadPrefixName = "ECS",
            ThreadCount = 3,
            MaxExpectedConcurrentJobs = 64,
            StrictAllocationMode = false
        };

        var scheduler = new JobScheduler(config);


        World.SharedJobScheduler = scheduler;
    }


    public Task StartAsync()
    {
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _world.Dispose();
    }
}
