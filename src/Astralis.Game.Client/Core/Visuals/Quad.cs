using System.Numerics;
using Arch.Core.Extensions;
using Astralis.Game.Client.Core.Textures;
using Astralis.Game.Client.Ecs.Components;
using Astralis.Game.Client.Ecs.GameObjects.Base;
using Astralis.Game.Client.Types;
using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Core.Visuals;

public class Quad : BaseGameObject
{
    public Vector3 Position { get; set; }

    private readonly TextureComponent _texture;
    private readonly Position3dComponent _position3D;
    private readonly QuadComponent _quadComponent;


    public Quad(Texture2d texture, Vector3 position, BlockFace face)
    {
        _texture = new TextureComponent(texture);
        Position = position;

        _position3D = new Position3dComponent(position);

        _quadComponent = new QuadComponent
        {
            Face = face
        };
    }


    protected override void AddComponents()
    {
        Entity.Add(_texture, _position3D, _quadComponent);
        base.AddComponents();
    }
}
