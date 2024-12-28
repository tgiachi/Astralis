using Astralis.Game.Client.Interfaces.Entities;

namespace Astralis.Game.Client.Components.Ecs;

public interface IImGuiComponent : IGameObject
{
    void Render();
}
