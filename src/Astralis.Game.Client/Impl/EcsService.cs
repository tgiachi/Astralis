using System.Numerics;
using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Astralis.Core.Attributes.Services;
using Astralis.Core.Interfaces.Services;
using Astralis.Core.Server.Events.Engine;
using Astralis.Game.Client.Components;
using Astralis.Game.Client.Core.Buffer;
using Astralis.Game.Client.Data.Events.Ecs;
using Astralis.Game.Client.Ecs.GameObjects;
using Astralis.Game.Client.Ecs.GameObjects.Debugger;
using Astralis.Game.Client.Ecs.Interfaces;
using Astralis.Game.Client.Interfaces.Entities;
using Astralis.Game.Client.Interfaces.Services;
using Astralis.Game.Client.Systems;
using ImGuiNET;
using Schedulers;
using Serilog;
using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Impl;

public class EcsService : IEcsService
{
    private readonly ILogger _logger = Log.ForContext<EcsService>();

    private readonly IEventBusService _eventBusService;
    private Group<double> _deltaTimeGroup;
    private Group<GL> _renderGroup;
    private readonly World _world;

    public EcsService(
        IEventBusService eventBusService
    )
    {
        _world = World.Create();
        _eventBusService = eventBusService;

        _eventBusService.Subscribe<AddEcsEntityEvent>(OnAddEcsEntity);


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
    }

    private void OnAddEcsEntity(AddEcsEntityEvent obj)
    {
        AddEntity(obj.GameObject);
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

    private void InitGroups()
    {
        _renderGroup = new Group<GL>(
            "render_group",
            new RenderSystem(_world),
            new TextRenderSystem(_world, AstralisGameInstances.OpenGlContext),
            new Texture2dRenderSystem(_world),
            new ImguiRenderSystem(_world),
            new DebugRenderSystem(_world)
        );
        _renderGroup.Initialize();

        _deltaTimeGroup = new Group<double>(
            "delta_time_group",
            new UpdateSystem(_world)
        );

        _deltaTimeGroup.Initialize();
    }

    private void OnEngineStarted(EngineStartedEvent obj)
    {
        _logger.Information("Starting ECS service...");

        AstralisGameInstances.OpenGlContext.OnUpdateEvent += OnUpdate;
        AstralisGameInstances.OpenGlContext.OnRenderEvent += OnRender;

        InitGroups();


        AddEntity(new TextGameObject("Ciao, sono tommy la version e' {app_version}", 500, 100));
        AddEntity(
            new ImGuiGameObject(
                () =>
                {
                    ImGui.Begin("Test 123");
                    ImGui.Text("Hello, world!");
                    ImGui.End();
                }
            )
        );

        AddEntity(new DebugMemoryGameObject());

        AddEntity(new PlayerGameObject(AstralisGameInstances.OpenGlContext));

        // try
        // {
        AddEntity(new Texture2dGameObject("grass_side", new Vector2(100, 100)));
        // }
        // catch (Exception e)
        // {
        //     _logger.Error(e, "Failed to create texture2d game object");
        // }
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
        AstralisGameInstances.OpenGlContext.OnUpdateEvent -= OnUpdate;
        AstralisGameInstances.OpenGlContext.OnRenderEvent -= OnRender;
        _deltaTimeGroup?.Dispose();
        _renderGroup?.Dispose();
        _world.Dispose();
    }

    public void AddEntity<TGameObject>(TGameObject gameObject) where TGameObject : IGameObject
    {
        var entity = _world.Create();

        gameObject.Initialize(entity);

        _logger.Debug("Created entity id: {EntityId} of type: {Type}", entity.Id, gameObject.GetType().Name);
    }
}
