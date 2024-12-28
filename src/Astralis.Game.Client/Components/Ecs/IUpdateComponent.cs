using Astralis.Game.Client.Interfaces.Entities;

namespace Astralis.Game.Client.Components.Ecs;


public interface IUpdateComponent : IGameObject
{
    public void Update(double deltaTime);
}
