using Astralis.Game.Client.Core.Utils;
using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Core.Buffer;

public class VertexArrayObject
{
    private readonly GL _gl;
    private readonly uint _handle;
    private readonly int _stride;

    public VertexArrayObject( GL gl, int stride)
    {
        _gl = gl;
        if (stride <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stride));
        }

        _stride = stride;

        _gl.GenVertexArrays(1, out _handle);
        GLUtility.CheckError(_gl);
    }

    public void Dispose()
    {
        _gl.DeleteVertexArray(_handle);
        GLUtility.CheckError(_gl);
    }

    public void Bind()
    {
        _gl.BindVertexArray(_handle);
        GLUtility.CheckError(_gl);
    }

    public unsafe void VertexAttribPointer(int location, int size, VertexAttribPointerType type, bool normalized, int offset)
    {
        _gl.EnableVertexAttribArray((uint)location);
        _gl.VertexAttribPointer((uint)location, size, type, normalized, (uint)_stride, (void*)offset);
        GLUtility.CheckError(_gl);
    }
}
