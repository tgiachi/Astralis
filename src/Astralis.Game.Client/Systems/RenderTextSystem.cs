using Arch.Core;
using Arch.System;
using Astralis.Game.Client.Interfaces.Ecs.Components;
using Astralis.Game.Client.Interfaces.Services;
using FontStash.NET;
using FontStash.NET.GL.Legacy;
using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Systems;

public class RenderTextSystem : BaseSystem<World, GL>
{
    private QueryDescription _desc = new QueryDescription().WithAll<ITextComponent>();
    private readonly IOpenGlContext _context;
    private readonly GLFons _font;
    private readonly Fontstash _fs;
    private readonly int _defaultFontId;

    public RenderTextSystem(World world, IOpenGlContext context) : base(world)
    {
        _context = context;
        _fs = new Fontstash(
            new FonsParams()
            {
                flags = (byte)FonsFlags.ZeroTopleft,
                height = 512,
                width = 512
            }
        );
        _defaultFontId = _fs.AddFont(
            "pixel",
            Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Fonts", "PixelOperator.ttf")
        );
    }

    public override void Update(in GL t)
    {
        World.Query(
            in _desc,
            (ref ITextComponent text) =>
            {
                _fs.SetSize(32);
                _fs.SetFont(_defaultFontId);
                _fs.DrawText(text.Position.X, text.Position.Y, text.Text);
            }
        );
    }
}
