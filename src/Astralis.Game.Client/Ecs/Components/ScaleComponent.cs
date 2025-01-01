using System.Numerics;

namespace Astralis.Game.Client.Ecs.Components;

public class ScaleComponent
{
    public Vector2 Size { get; set; }

    public ScaleComponent(float x, float y)
    {
        Size = new Vector2(x, y);
    }

}
