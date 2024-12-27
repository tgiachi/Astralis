using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Astralis.Core.Attributes.Services;
using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Events.Engine;
using Astralis.Game.Client.Components;
using Astralis.Game.Client.Components.Ecs;
using Astralis.Game.Client.Interfaces.Services;
using Astralis.Game.Client.Systems;
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
            ThreadCount = 0,
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
        _renderGroup?.BeforeUpdate(in gl);
        _renderGroup?.Update(in gl);
        _renderGroup?.AfterUpdate(in gl);
    }

    private void OnUpdate(double deltaTime)
    {
        _deltaTimeGroup?.BeforeUpdate(in deltaTime);
        _deltaTimeGroup?.Update(in deltaTime);
        _deltaTimeGroup?.AfterUpdate(in deltaTime);
    }

    private void OnEngineStarted(EngineStartedEvent obj)
    {
        _logger.Information("Starting ECS service...");

        _renderGroup = new Group<GL>(
            "render_group",
            new TextRenderSystem(_world, _openGlContext),
            new ImguiRenderSystem(_world)
        );
        _renderGroup.Initialize();

        _deltaTimeGroup = new Group<double>(
            "delta_time_group",
            new UpdateSystem(_world)
        );
        var text = new DefaultTextComponent("Hello, World!", 500, 100);
        var entity = CreateEntity(text);

        entity.Add((IUpdateComponent)text, (ITextComponent)text);

        IImGuiComponent imgui = new ImGuiDefaultComponent();

        var entity2 = _world.Create();

        entity2.Add(imgui);

        var isA = entity2.Has(typeof(IImGuiComponent));
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

    public Entity CreateEntity(params object[] components)
    {
        var entity = _world.Create();

        foreach (var component in components)
        {
            entity.Add(component);
        }

        _logger.Debug("Created entity {EntityId}", entity.Id);
        return entity;
    }
}
