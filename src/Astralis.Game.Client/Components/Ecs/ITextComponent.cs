using System.Numerics;

namespace Astralis.Game.Client.Components.Ecs;

public interface ITextComponent : IPosition2dComponent
{
    public float FontSize { get; set; }
    public string Text { get; set; }
    public Vector4 Color { get; set; }
}
