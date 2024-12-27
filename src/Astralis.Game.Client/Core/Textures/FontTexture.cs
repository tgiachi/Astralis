using System.Drawing;
using Astralis.Game.Client.Core.Utils;
using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Core.Textures;

public unsafe class FontTexture : IDisposable
{
    private readonly GL _gl;
    private readonly uint _handle;
    public readonly int Width;
    public readonly int Height;


    public  FontTexture(GL gl, int width, int height)
    {
        _gl = gl;
        Width = width;
        Height = height;

        _handle = gl.GenTexture();
        GLUtility.CheckError(gl);
        Bind();

        //Reserve enough memory from the gpu for the whole image
        gl.TexImage2D(
            TextureTarget.Texture2D,
            0,
            InternalFormat.Rgba8,
            (uint)width,
            (uint)height,
            0,
            PixelFormat.Rgba,
            PixelType.UnsignedByte,
            null
        );
        GLUtility.CheckError(gl);

        SetParameters();
    }

    private void SetParameters()
    {
        //Setting some texture perameters so the texture behaves as expected.
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
        GLUtility.CheckError(_gl);

        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
        GLUtility.CheckError(_gl);

        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
        GLUtility.CheckError(_gl);

        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
        GLUtility.CheckError(_gl);

        //Generating mipmaps.
        _gl.GenerateMipmap(TextureTarget.Texture2D);
        GLUtility.CheckError(_gl);
    }

    public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
    {
        //When we bind a texture we can choose which textureslot we can bind it to.
        _gl.ActiveTexture(textureSlot);
        GLUtility.CheckError(_gl);

        _gl.BindTexture(TextureTarget.Texture2D, _handle);
        GLUtility.CheckError(_gl);
    }

    public void Dispose()
    {
        //In order to dispose we need to delete the opengl handle for the texure.
        _gl.DeleteTexture(_handle);
        GLUtility.CheckError(_gl);
    }

    public unsafe void SetData(Rectangle bounds, byte[] data)
    {
        Bind();
        fixed (byte* ptr = data)
        {
            _gl.TexSubImage2D(
                target: TextureTarget.Texture2D,
                level: 0,
                xoffset: bounds.Left,
                yoffset: bounds.Top,
                width: (uint)bounds.Width,
                height: (uint)bounds.Height,
                format: PixelFormat.Rgba,
                type: PixelType.UnsignedByte,
                pixels: ptr
            );
            GLUtility.CheckError(_gl);
        }
    }
}
