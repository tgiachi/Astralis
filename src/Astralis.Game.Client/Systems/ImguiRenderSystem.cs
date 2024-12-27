using Arch.Core;
using Arch.System;
using Astralis.Game.Client.Components.Ecs;
using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Systems;

public class ImguiRenderSystem : BaseSystem<World, GL>
{
    private readonly QueryDescription _desc = new QueryDescription().WithAny<IImGuiComponent>();

    public ImguiRenderSystem(World world) : base(world)
    {
    }

    public override void Update(in GL t)
    {
        World.Query(
            in _desc,
            (ref IImGuiComponent text) =>
            {
                text.Render();
            }
        );

        base.Update(in t);
    }
}
