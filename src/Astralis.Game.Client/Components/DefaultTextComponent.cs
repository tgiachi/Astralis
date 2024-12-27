using System.Numerics;
using Astralis.Game.Client.Components.Ecs;

namespace Astralis.Game.Client.Components;

public class DefaultTextComponent : ITextComponent
{
    public Vector2 Position { get; set; }
    public float FontSize { get; set; }
    public string Text { get; set; }
    public Vector4 Color { get; set; }

    public DefaultTextComponent(string text, float x, float y, float fontSize = 32)
    {

        
    }



}
