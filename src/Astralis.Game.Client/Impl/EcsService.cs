using Arch.Core;
using Arch.System;
using Astralis.Core.Attributes.Services;
using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Events.Engine;

using Astralis.Game.Client.Interfaces.Services;

using Schedulers;
using Serilog;
using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Impl;

[OrionSystemService(1)]
public class EcsService : IEcsService
{
    private readonly ILogger _logger = Log.ForContext<EcsService>();
    private readonly IOpenGlContext _openGlContext;
    private readonly IEventBusService _eventBusService;
    private Group<double> _deltaTimeGroup;
    private Group<GL> _renderGroup;
    private readonly World _world;

    public EcsService(
        World world, IEventBusService eventBusService, IOpenGlContext openGlContext
    )
    {
        _world = world;
        _eventBusService = eventBusService;
        _openGlContext = openGlContext;


        var config = new JobScheduler.Config
        {
            ThreadPrefixName = "ECS",
            ThreadCount = 3,
            MaxExpectedConcurrentJobs = 64,
            StrictAllocationMode = false
        };


        var scheduler = new JobScheduler(config);
        World.SharedJobScheduler = scheduler;

        _eventBusService.Subscribe<EngineStartedEvent>(OnEngineStarted);

        _openGlContext.OnUpdateEvent += OnUpdate;
        _openGlContext.OnRenderEvent += OnRender;
    }

    private void OnRender(double deltaTime, GL gl)
    {
        _renderGroup?.BeforeUpdate(gl);
        _renderGroup?.Update(gl);
        _renderGroup?.AfterUpdate(gl);
    }

    private void OnUpdate(double deltaTime)
    {
        _deltaTimeGroup?.BeforeUpdate(deltaTime);
        _deltaTimeGroup?.Update(deltaTime);
        _deltaTimeGroup?.AfterUpdate(deltaTime);
    }

    private void OnEngineStarted(EngineStartedEvent obj)
    {
        _logger.Information("Starting ECS service...");
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
        _openGlContext.OnUpdateEvent -= OnUpdate;
        _openGlContext.OnRenderEvent -= OnRender;
        _deltaTimeGroup?.Dispose();
        _renderGroup?.Dispose();
        _world.Dispose();
    }
}
