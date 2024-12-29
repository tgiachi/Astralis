using System.Numerics;

namespace Astralis.Game.Client.Ecs.Components;

public record TextComponent
{
    public string FontName { get; set; }
    public float FontSize { get; set; }
    public string Text { get; set; }
    public Vector4 Color { get; set; }
    public float Rotation { get; set; }


}
