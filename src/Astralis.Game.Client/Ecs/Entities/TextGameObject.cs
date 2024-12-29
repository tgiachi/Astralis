using System.Numerics;
using Arch.Core.Extensions;
using Astralis.Core.Server.Interfaces.Services.System;

using Astralis.Game.Client.Ecs.Components;
using Astralis.Game.Client.Ecs.Entities.Base;


namespace Astralis.Game.Client.Ecs.Entities;

public class TextGameObject : BaseGameObject
{
    private readonly TextComponent _textComponent;

    private string _text;

    private readonly IVariablesService  _variablesService;

    private readonly Position2dComponent _position2dComponent;

    public TextGameObject(string text, float x, float y, float fontSize = 16, string fontName = "Default")
    {
        _variablesService = AstralisGameInstances.VariablesService();
        _text = text;
        _textComponent = new TextComponent()
        {
            Text = _variablesService.TranslateText(text),
            FontSize = fontSize,
            FontName = fontName,
            Color = new Vector4(1, 1, 1, 255),
            Rotation = 0
        };
        _position2dComponent = new Position2dComponent(x, y);
        
    }

    public void SetText(string text)
    {
        _text = text;
    }

    protected override void AddComponents()
    {
        Entity.Add(_textComponent, _position2dComponent);

        base.AddComponents();
    }

    public override void Update(double deltaTime)
    {
        _textComponent.Text = _variablesService.TranslateText(_text);
        base.Update(deltaTime);
    }
}
