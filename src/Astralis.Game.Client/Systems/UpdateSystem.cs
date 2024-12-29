using Arch.Core;
using Arch.System;
using Astralis.Game.Client.Ecs.Interfaces;

namespace Astralis.Game.Client.Systems;

public class UpdateSystem : BaseSystem<World, double>
{
    private readonly QueryDescription _desc = new QueryDescription().WithAny<IDoUpdate>();

    public UpdateSystem(World world) : base(world)
    {
    }

    public override void Update(in double t)
    {
        var d = t;
        World.Query(
            in _desc,
            (ref IDoUpdate updateComponent) =>
            {
                updateComponent.Update(d);
            }
        );

        base.Update(in t);
    }
}
