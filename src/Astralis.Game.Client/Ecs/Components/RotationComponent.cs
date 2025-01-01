namespace Astralis.Game.Client.Ecs.Components;

public class RotationComponent
{
    public float Rotation { get; set; }

    public RotationComponent(float rotation)
    {
        Rotation = rotation;
    }

    public RotationComponent()
    {

    }
}
