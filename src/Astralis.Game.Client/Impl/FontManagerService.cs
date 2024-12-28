using Astralis.Core.Extensions;
using Astralis.Game.Client.Interfaces.Services;
using Astralis.Game.Client.Types;
using FontStashSharp;
using Serilog;

namespace Astralis.Game.Client.Impl;

public class FontManagerService : IFontManagerService
{
    private readonly ILogger _logger = Log.ForContext<FontManagerService>();

    private readonly Dictionary<string, FontSystem> _fonts = new();

    public FontManagerService()
    {
        ScanForFonts();
    }

    private void ScanForFonts()
    {
        var fonts = AstralisGameInstances.AssetDirectories.ScanDirectory(AssetDirectoryType.Fonts, "*.ttf");

        foreach (var font in fonts)
        {
            var fontName = Path.GetFileNameWithoutExtension(font).ToSnakeCase();
            _logger.Information("Loading font {FontName} from {Font}", fontName, font);
            _fonts.Add(
                fontName,
                new FontSystem(
                    new FontSystemSettings
                    {
                        FontResolutionFactor = 2,
                        KernelWidth = 2,
                        KernelHeight = 2
                    }
                )
            );
            _fonts[fontName].AddFont(File.ReadAllBytes(font));
        }
    }

    public DynamicSpriteFont GetFont(string fontName, float size = 32)
    {
        fontName = fontName.ToSnakeCase();
        if (!_fonts.TryGetValue(fontName, out var font))
        {
            _logger.Error("Font {FontName} not found", fontName);
            throw new Exception($"Font {fontName} not found");
        }

        return font.GetFont(size);
    }

    public void Dispose()
    {
        foreach (var font in _fonts.Values)
        {
            font.Dispose();
        }
    }
}
