using Arch.Core.Extensions;
using Astralis.Game.Client.Components.Entities;
using Astralis.Game.Client.Ecs.Components;
using Astralis.Game.Client.Ecs.Entities.Base;

namespace Astralis.Game.Client.Ecs.Entities;

public class ImGuiGameObject : BaseGameObject, IImGuiComponent
{
    private readonly Action _renderAction;

    public ImGuiGameObject(Action renderAction)
    {
        _renderAction = renderAction;
    }

    public virtual void Render()
    {
        _renderAction();
    }

    protected override void AddComponents()
    {
        base.AddComponents();

        Entity.Add(new ImGuiComponent(this));
    }
}
