using System.Numerics;

namespace Astralis.Game.Client.Interfaces.Ecs.Components;

public interface ITextComponent : I2dPositionComponent, IUpdateComponent
{
    string Text { get; }

    Vector4 Color { get; }
}
