using Arch.Core;
using Arch.System;
using Astralis.Game.Client.Components.Ecs;

namespace Astralis.Game.Client.Systems;

public class UpdateSystem : BaseSystem<World, double>
{
    private readonly QueryDescription _desc = new QueryDescription().WithAny<IUpdateComponent>();

    public UpdateSystem(World world) : base(world)
    {
    }

    public override void Update(in double t)
    {
        var d = t;
        World.Query(
            in _desc,
            (ref IUpdateComponent updateComponent) =>
            {
                updateComponent.Update(d);
            }
        );

        base.Update(in t);
    }
}
