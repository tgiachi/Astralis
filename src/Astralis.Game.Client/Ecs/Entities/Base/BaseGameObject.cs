using Arch.Core;
using Arch.Core.Extensions;
using Astralis.Game.Client.Components.Entities;
using Astralis.Game.Client.Ecs.Components;
using Astralis.Game.Client.Ecs.Interfaces;
using Astralis.Game.Client.Interfaces.Entities;

namespace Astralis.Game.Client.Ecs.Entities.Base;

public abstract class BaseGameObject : IGameObject, IDoUpdate
{
    public int Id { get; set; }
    public Entity Entity { get; set; }

    public void Initialize(Entity entity)
    {
        Entity = entity;
        Id = entity.Id;

        AddComponents();
    }

    public virtual void Update(double deltaTime)
    {
    }

    protected virtual void AddComponents()
    {
        if (this is IDoUpdate doUpdate)
        {
            Entity.Add(new DoUpdateComponent(doUpdate));
        }

        if (this is IDoRender doRender)
        {
            Entity.Add(new DoRenderComponent(doRender));
        }

        if (this is IDebuggableComponent debuggableComponent)
        {
            Entity.Add(new DebuggableComponent(debuggableComponent));
        }
    }
}
