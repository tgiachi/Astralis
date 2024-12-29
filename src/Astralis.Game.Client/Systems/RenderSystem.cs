using Arch.Core;
using Arch.System;
using Astralis.Game.Client.Ecs.Components;
using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Systems;

public class RenderSystem : BaseSystem<World, GL>
{
    private readonly QueryDescription _desc = new QueryDescription().WithAny<DoRenderComponent>();

    public RenderSystem(World world) : base(world)
    {
    }

    public override void Update(in GL t)
    {
        var rnd = t;
        World.Query(
            in _desc,
            (ref DoRenderComponent render) => { render.Render.Render(rnd); }
        );

        base.Update(in t);
    }
}
