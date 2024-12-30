using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Core.Buffer;

public class ABufferObject<TDataType> : IDisposable
    where TDataType : unmanaged
{
    private readonly uint _handle;
    private readonly BufferTargetARB _bufferType;
    private readonly GL _gl;
    private bool disposed = false;

    public unsafe ABufferObject(
        GL gl, Span<TDataType> data, BufferTargetARB bufferType, BufferUsageARB usage = BufferUsageARB.StaticDraw
    )
    {
        _gl = gl;
        _bufferType = bufferType;


        _handle = _gl.GenBuffer();
        Bind(bufferType);
        fixed (void* d = data)
        {
            _gl.BufferData(bufferType, (nuint)(data.Length * sizeof(TDataType)), d, usage);
        }
    }

    public unsafe ABufferObject(
        GL gl, int nbVertex, BufferTargetARB bufferType, BufferUsageARB bufferUsageArb = BufferUsageARB.DynamicCopy
    )
    {
        _gl = gl;
        _bufferType = bufferType;

        _handle = gl.GenBuffer();
        Bind(bufferType);
        gl.BufferData(bufferType, (nuint)(nbVertex * sizeof(TDataType)), null, bufferUsageArb);
    }

    public void SendData(ReadOnlySpan<TDataType> data, nint offset)
    {
        Bind(_bufferType);
        _gl.BufferSubData(_bufferType, offset, data);
    }


    public void Bind(BufferTargetARB bufferType)
    {
        _gl.BindBuffer(bufferType, _handle);
    }

    public unsafe TDataType GetData()
    {
        _gl.GetNamedBufferSubData<TDataType>(_handle, 0, (uint)sizeof(TDataType), out var countCompute);
        return countCompute;
    }

    ~ABufferObject()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _gl.DeleteBuffer(_handle);
            }

            disposed = true;
        }
    }
}
