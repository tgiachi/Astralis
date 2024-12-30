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

    public unsafe BufferObject(
        GL gl, int nbVertex, BufferTargetARB bufferType, BufferUsageARB bufferUsageArb = BufferUsageARB.DynamicCopy
    )
    {
        _gl = gl;
        _bufferType = bufferType;

        _handle = gl.GenBuffer();
        Bind(bufferType);
        gl.BufferData(bufferType, (nuint)(nbVertex * sizeof(T)), null, bufferUsageArb);
    }

    public unsafe BufferObject(
        GL gl, Span<T> data, BufferTargetARB bufferType, BufferUsageARB usage = BufferUsageARB.StaticDraw
    )
    {
        _gl = gl;
        _bufferType = bufferType;


        _handle = _gl.GenBuffer();
        Bind(bufferType);
        fixed (void* d = data)
        {
            _gl.BufferData(bufferType, (nuint)(data.Length * sizeof(T)), d, usage);
        }
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

    public void SendData(ReadOnlySpan<T> data, nint offset)
    {
        Bind(_bufferType);
        _gl.BufferSubData(_bufferType, offset, data);
    }

    public void Bind(BufferTargetARB bufferType)
    {
        _gl.BindBuffer(bufferType, _handle);
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
