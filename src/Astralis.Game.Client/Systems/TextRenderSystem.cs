using System.Numerics;
using Arch.Core;
using Arch.System;

using Astralis.Game.Client.Components.Ecs;
using Astralis.Game.Client.Core.Text;
using Astralis.Game.Client.Interfaces.Services;
using FontStashSharp;

using Silk.NET.OpenGL;


namespace Astralis.Game.Client.Systems;

public class TextRenderSystem : BaseSystem<World, GL>
{
    private readonly QueryDescription _desc = new QueryDescription().WithAny<IPosition2dComponent, ITextComponent>();

    private readonly TextRenderer _renderer;

    public TextRenderSystem(World world, IOpenGlContext context) : base(world)
    {
        _renderer = new TextRenderer(context);


    }

    public override void Update(in GL t)
    {
        _renderer.Begin();
        World.Query(
            in _desc,
            (ref ITextComponent text) =>
            {
                var font = AstralisGameInstances.FontManagerService().GetFont(text.FontName, text.FontSize);
                var size = font.MeasureString(text.Text, Vector2.One);
                var origin = new Vector2(size.X / 2.0f, size.Y / 2.0f);
                font.DrawText(
                    _renderer,
                    text.Text,
                    text.Position,
                    new FSColor(text.Color.X, text.Color.Y, text.Color.Z, text.Color.W),
                    text.Rotation,
                    origin,
                    Vector2.One
                );
            }
        );


        _renderer.End();
    }
}
