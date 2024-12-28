using FontStashSharp;

namespace Astralis.Game.Client.Interfaces.Services;

public interface IFontManagerService : IDisposable
{
    DynamicSpriteFont GetFont(string fontName, float size = 32);
}
