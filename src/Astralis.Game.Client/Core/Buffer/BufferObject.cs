using System.Runtime.InteropServices;
using Astralis.Game.Client.Core.Utils;
using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Core.Buffer;

public class BufferObject<T> : IDisposable where T : unmanaged
{
    private readonly uint _handle;
    private readonly GL _gl;
    private readonly BufferTargetARB _bufferType;
    private readonly int _size;

    public unsafe BufferObject(GL gl, int size, BufferTargetARB bufferType, bool isDynamic)
    {
        _gl = gl;
        _bufferType = bufferType;
        _size = size;

        _handle = gl.GenBuffer();
        GLUtility.CheckError(gl);

        Bind();

        var elementSizeInBytes = Marshal.SizeOf<T>();
        gl.BufferData(
            bufferType,
            (nuint)(size * elementSizeInBytes),
            null,
            isDynamic ? BufferUsageARB.StreamDraw : BufferUsageARB.StaticDraw
        );
        GLUtility.CheckError(gl);
    }

    public void Bind()
    {
        _gl.BindBuffer(_bufferType, _handle);
        GLUtility.CheckError(_gl);
    }

    public void Dispose()
    {
        _gl.DeleteBuffer(_handle);
        GLUtility.CheckError(_gl);
    }

    public unsafe void SetData(T[] data, int startIndex, int elementCount)
    {
        Bind();

        fixed (T* dataPtr = &data[startIndex])
        {
            var elementSizeInBytes = sizeof(T);

            _gl.BufferSubData(_bufferType, 0, (nuint)(elementCount * elementSizeInBytes), dataPtr);
            GLUtility.CheckError(_gl);
        }
    }
}
