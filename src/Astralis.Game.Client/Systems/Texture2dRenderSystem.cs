using System.Numerics;
using Arch.Core;
using Arch.System;
using Astralis.Game.Client.Core.Text;
using Astralis.Game.Client.Ecs.Components;
using FontStashSharp;
using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Systems;

public class Texture2dRenderSystem : BaseSystem<World, GL>
{
    private readonly QueryDescription _desc = new QueryDescription()
        .WithAll<TextureComponent, Position2dComponent, ScaleComponent>();

    private readonly TextRenderer _renderer;

    public Texture2dRenderSystem(World world) : base(world)
    {
        _renderer = new TextRenderer(AstralisGameInstances.OpenGlContext);
    }

    public override unsafe void Update(in GL t)
    {
        var gl = t;


        _renderer.Begin();

        World.Query(
            in _desc,
            (
                ref TextureComponent texture, ref Position2dComponent position, ref ScaleComponent scale
            ) =>
            {
                _renderer.DrawTexture(
                    texture.Texture,
                    position.Position,
                    new Vector2(texture.Texture.Width * scale.Size.X, texture.Texture.Height * scale.Size.Y),
                    FSColor.White
                );
            }
        );

        _renderer.End();

        base.Update(in t);
    }
}
