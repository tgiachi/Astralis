using System.Numerics;


namespace Astralis.Game.Client.Components.Ecs;

public interface ITextComponent : IPosition2dComponent, IUpdateComponent
{
    string FontName { get; set; }
    float FontSize { get; set; }
    string Text { get; set; }
    Vector4 Color { get; set; }
    float Rotation { get; set; }
}
