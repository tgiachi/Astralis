using System.Numerics;
using Astralis.Game.Client.Components.Ecs;

namespace Astralis.Game.Client.Components;

public class DefaultTextComponent : ITextComponent
{
    public Vector2 Position { get; set; }
    public float FontSize { get; set; }
    public string Text { get; set; }
    public Vector4 Color { get; set; }

    public float Rotation { get; set; } = 0;

    public DefaultTextComponent(string text, float x, float y, float fontSize = 16)
    {
        Text = text;
        Position = new Vector2(x, y);
        FontSize = fontSize;
        Color = new Vector4(1, 1, 1, 255);
    }


    public virtual void Update(double deltaTime)
    {
    }
}
