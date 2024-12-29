using Arch.Core;

namespace Astralis.Game.Client.Interfaces.Entities;

public interface IGameObject
{
    int Id { get; set; }
    Entity Entity { get; set; }
    void Initialize(Entity entity);

}
