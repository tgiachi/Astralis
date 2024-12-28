using System.Numerics;
using Astralis.Game.Client.Interfaces.Entities;

namespace Astralis.Game.Client.Components.Ecs;

public interface IPosition2dComponent : IGameObject
{
    public Vector2 Position { get; set; }
}
