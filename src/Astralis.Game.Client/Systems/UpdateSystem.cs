using Arch.Core;
using Arch.System;
using Astralis.Game.Client.Ecs.Components;
using Astralis.Game.Client.Ecs.Interfaces;

namespace Astralis.Game.Client.Systems;

public class UpdateSystem : BaseSystem<World, double>
{
    private readonly QueryDescription _desc = new QueryDescription().WithAny<DoUpdateComponent>();

    public UpdateSystem(World world) : base(world)
    {
    }

    public override void Update(in double t)
    {
        var d = t;
        World.Query(
            in _desc,
            (ref DoUpdateComponent updateComponent) =>
            {
                updateComponent.Update.Update(d);
            }
        );

        base.Update(in t);
    }
}
