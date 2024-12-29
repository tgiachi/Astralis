using Arch.Core;
using Arch.System;
using Astralis.Game.Client.Components;
using Astralis.Game.Client.Ecs.Components;
using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Systems;

public class ImguiRenderSystem : BaseSystem<World, GL>
{
    private readonly QueryDescription _desc = new QueryDescription().WithAny<ImGuiComponent>();

    public ImguiRenderSystem(World world) : base(world)
    {
    }

    public override void Update(in GL t)
    {
        World.Query(
            in _desc,
            (ref ImGuiComponent text) => { text.Obj.Render(); }
        );

        base.Update(in t);
    }
}
