using Astralis.Game.Client.Interfaces.Entities;

namespace Astralis.Game.Client.Components.Ecs;

public interface IDebuggableComponent : IGameObject
{
    string Name { get; }
    void DebugRender();
}
