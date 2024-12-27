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
    private readonly QueryDescription _desc = new QueryDescription().WithAny<DefaultTextComponent>();
    private readonly IOpenGlContext _context;
    private readonly TextRenderer _renderer;
    private readonly FontSystem _fontSystem;

    public TextRenderSystem(World world, IOpenGlContext context) : base(world)
    {
        _context = context;
        _renderer = new TextRenderer(_context.Gl);

        var settings = new FontSystemSettings
        {
            FontResolutionFactor = 2,
            KernelWidth = 2,
            KernelHeight = 2
        };

        _fontSystem = new FontSystem(settings);
        _fontSystem.AddFont(
            File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Fonts", "PixelOperator.ttf"))
        );
    }


    public override void Update(in GL t)
    {
        var text = "The quick いろは brown\nfox にほへ jumps over\nt🙌h📦e l👏a👏zy dog";
        var scale = new Vector2(2, 2);

        var font = _fontSystem.GetFont(32);

        var size = font.MeasureString(text, scale);
        var origin = new Vector2(size.X / 2.0f, size.Y / 2.0f);
        _renderer.Begin();
        World.Query(
            in _desc,
            (ref DefaultTextComponent text) => { }
        );
        try
        {
            font.DrawText(_renderer, text, new Vector2(10, 10), FSColor.Black, 0.0f, origin, scale);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        _renderer.End();
    }
}
