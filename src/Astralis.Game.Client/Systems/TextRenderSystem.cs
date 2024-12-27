using System.Numerics;
using Arch.Core;
using Arch.System;
using Astralis.Game.Client.Components;
using Astralis.Game.Client.Components.Ecs;
using Astralis.Game.Client.Core.Text;
using Astralis.Game.Client.Interfaces.Services;
using FontStashSharp;
using ImGuiNET;
using Silk.NET.OpenGL;


namespace Astralis.Game.Client.Systems;

public class TextRenderSystem : BaseSystem<World, GL>
{
    private readonly QueryDescription _desc = new QueryDescription().WithAny<IPosition2dComponent, ITextComponent>();
    private readonly IOpenGlContext _context;
    private readonly TextRenderer _renderer;
    private readonly FontSystem _fontSystem;
    private static float _rads = 0.0f;

    public TextRenderSystem(World world, IOpenGlContext context) : base(world)
    {
        _context = context;
        _renderer = new TextRenderer(_context);

        var settings = new FontSystemSettings
        {
            FontResolutionFactor = 2,
            KernelWidth = 2,
            KernelHeight = 2
        };

        _fontSystem = new FontSystem(settings);
        var font = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Fonts", "PixelOperator.ttf"));
        _fontSystem.AddFont(font);
    }


    public override void Update(in GL t)
    {
        var text = "Hello World!";
        var scale = new Vector2(2, 2);

        var font = _fontSystem.GetFont(32);

        var size = font.MeasureString(text, scale);
        var origin = new Vector2(size.X / 2.0f, size.Y / 2.0f);


        _renderer.Begin();
        World.Query(
            in _desc,
            (ref ITextComponent text) =>
            {
                font.DrawText(_renderer, text.Text, text.Position, new FSColor(text.Color.X, text.Color.Y, text.Color.Z, text.Color.W), 0, origin, scale);
            }
        );

        font.DrawText(_renderer, text, new Vector2(400, 400), FSColor.LightCoral, 0, origin, scale);

        _renderer.End();
    }
}
