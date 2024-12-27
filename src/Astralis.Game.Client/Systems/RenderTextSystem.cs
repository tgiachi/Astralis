using Arch.Core;
using Arch.System;
using Astralis.Game.Client.Components;
using Astralis.Game.Client.Components.Ecs;
using Astralis.Game.Client.Interfaces.Services;
using ImGuiNET;
using Silk.NET.OpenGL;


namespace Astralis.Game.Client.Systems;

public class RenderTextSystem : BaseSystem<World, GL>
{
    private readonly QueryDescription _desc = new QueryDescription().WithAny<DefaultTextComponent>();
    private readonly IOpenGlContext _context;


    public RenderTextSystem(World world, IOpenGlContext context) : base(world)
    {
        _context = context;
    }

    public override void Update(in GL t)
    {
        World.Query(
            in _desc,
            (ref DefaultTextComponent text) => { }
        );
    }
}
