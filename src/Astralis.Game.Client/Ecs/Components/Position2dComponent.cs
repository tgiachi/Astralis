using System.Numerics;

namespace Astralis.Game.Client.Ecs.Components;

public record Position2dComponent
{
    public Vector2 Position { get; set; }

    public Position2dComponent(float x, float y)
    {
        Position = new Vector2(x, y);
    }
}
