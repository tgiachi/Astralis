using Astralis.Game.Client.Core.Utils;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Rectangle = System.Drawing.Rectangle;

namespace Astralis.Game.Client.Core.Textures;

public unsafe class Texture2d : IDisposable
{
    private readonly GL _gl;
    private readonly uint _handle;
    public readonly int Width;
    public readonly int Height;


    public Texture2d(GL gl, int width, int height)
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

    public Texture2d(GL gl, Span<byte> data, uint width, uint height)
    {
        //Saving the gl instance.
        _gl = gl;

        //Setting the width and height of the texture.
        Width = (int)width;
        Height = (int)height;

        //Generating the opengl handle;
        _handle = this._gl.GenTexture();
        Bind();

        //We want the ability to create a texture using data generated from code aswell.
        fixed (void* d = &data[0])
        {
            //Setting the data of a texture.
            _gl.TexImage2D(
                TextureTarget.Texture2D,
                0,
                (int)InternalFormat.Rgba,
                width,
                height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                d
            );
            SetParameters();
        }
    }

    public Texture2d(GL gl, string path)
    {
        _gl = gl;
        _handle = gl.GenTexture();

        Bind();

        using var img = Image.Load<Rgba32>(path);
        img.Mutate(x => x.Flip(FlipMode.Vertical));

        gl.TexImage2D(
            TextureTarget.Texture2D,
            0,
            InternalFormat.Rgba8,
            (uint)img.Width,
            (uint)img.Height,
            0,
            PixelFormat.Rgba,
            PixelType.UnsignedByte,
            null
        );

        img.ProcessPixelRows(
            accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    fixed (void* data = accessor.GetRowSpan(y))
                    {
                        //Loading the actual image.
                        gl.TexSubImage2D(
                            TextureTarget.Texture2D,
                            0,
                            0,
                            y,
                            (uint)accessor.Width,
                            1,
                            PixelFormat.Rgba,
                            PixelType.UnsignedByte,
                            data
                        );
                    }
                }
            }
        );

        Width = img.Width;
        Height = img.Height;

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
