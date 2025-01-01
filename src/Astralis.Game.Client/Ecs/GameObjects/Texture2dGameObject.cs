using System.Collections.Immutable;
using System.Numerics;
using Arch.Core.Extensions;
using Astralis.Game.Client.Ecs.Components;
using Astralis.Game.Client.Ecs.GameObjects.Base;


namespace Astralis.Game.Client.Ecs.GameObjects;

public class Texture2dGameObject : BaseGameObject
{
    private readonly Position2dComponent _position2dComponent;
    private readonly ScaleComponent _scaleComponent;
    private readonly TextureComponent _textureComponent;


    public  Texture2dGameObject(string textureName, Vector2 position, Vector2 scale = default)
    {
        var gl = AstralisGameInstances.OpenGlContext.Gl;

        Name = textureName;

        if (scale == default)
        {
            scale = Vector2.One;
        }

        _position2dComponent = new Position2dComponent(position.X, position.Y);
        _scaleComponent = new ScaleComponent(scale.X, scale.Y);


        _textureComponent = new TextureComponent(AstralisGameInstances.TextureManagerService().GetTexture(textureName));
    }


    protected override void AddComponents()
    {
        Entity.Add(_textureComponent, _position2dComponent, _scaleComponent);
        base.AddComponents();
    }
}
