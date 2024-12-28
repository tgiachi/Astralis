using System.Numerics;
using Astralis.Core.Server.Interfaces.Services.System;
using Astralis.Game.Client.Components.Ecs;

namespace Astralis.Game.Client.Components;

public class TextComponent : ITextComponent
{
    public Vector2 Position { get; set; }
    public string FontName { get; set; }
    public float FontSize { get; set; }
    public string Text { get; set; }
    public Vector4 Color { get; set; }
    public float Rotation { get; set; } = 0;
    public string SourceText { get; set; }

    private readonly IVariablesService _variablesService;

    public TextComponent(string text, float x, float y, float fontSize = 8, string fontName = "Default")
    {
        _variablesService = AstralisGameInstances.VariablesService();
        SourceText = text;
        Text = text;
        Position = new Vector2(x, y);
        FontSize = fontSize;
        FontName = fontName;
        Color = new Vector4(1, 1, 1, 255);
    }


    public virtual void Update(double deltaTime)
    {
        Text = _variablesService.TranslateText(SourceText);
    }
}
