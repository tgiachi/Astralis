using System.Drawing;
using Astralis.Game.Client.Core.Textures;
using FontStashSharp.Interfaces;
using Silk.NET.OpenGL;


namespace Astralis.Game.Client.Core.Text;

public class Texture2DManager : ITexture2DManager
{
    private readonly GL _gl;

    public Texture2DManager(GL gl)
    {
        _gl = gl;
    }

    public object CreateTexture(int width, int height) => new Texture2d(_gl, width, height);

    public Point GetTextureSize(object texture)
    {
        var t = (Texture2d)texture;
        return new Point(t.Width, t.Height);
    }

    public void SetTextureData(object texture, Rectangle bounds, byte[] data)
    {
        var t = (Texture2d)texture;
        t.SetData(bounds, data);
    }
}
