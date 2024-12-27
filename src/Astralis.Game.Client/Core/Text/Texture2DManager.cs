using System.Drawing;
using Astralis.Game.Client.Core.Textures;
using FontStashSharp.Interfaces;
using Silk.NET.OpenGL;
using Texture = Astralis.Game.Client.Core.Textures.Texture;

namespace Astralis.Game.Client.Core.Text;

public class Texture2DManager : ITexture2DManager
{
    private readonly GL _gl;

    public Texture2DManager(GL gl)
    {
        _gl = gl;
    }

    public object CreateTexture(int width, int height) => new FontTexture(_gl, width, height);

    public Point GetTextureSize(object texture)
    {
        var t = (Texture)texture;
        return new Point(t.Width, t.Height);
    }

    public void SetTextureData(object texture, Rectangle bounds, byte[] data)
    {
        var t = (FontTexture)texture;
        t.SetData(bounds, data);
    }
}
