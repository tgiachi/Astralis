using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;


namespace Astralis.Game.Client.Core.Buffer;

public class AVertexArrayObject<TVertexType, TIndexType> : IDisposable
    where TVertexType : unmanaged
    where TIndexType : unmanaged
{
    private readonly uint _handle;
    private readonly GL _gl;
    private bool disposed = false;

    public AVertexArrayObject(GL gl, ABufferObject<TVertexType> vbo, ABufferObject<TIndexType>? ebo = null)
    {
        _gl = gl;

        _handle = _gl.GenVertexArray();
        Bind();
        vbo.Bind(BufferTargetARB.ArrayBuffer);
        ebo?.Bind(BufferTargetARB.ElementArrayBuffer);
    }

    /**
     * <summary>Bind a vertex attribute to a field of the vertex type
     *  the type of the field is converted to a float vector
     * </summary>
     * <param name="index">The index of the vertex attribute</param>
     * <param name="count">The number of components of the vertex attribute</param>
     * <param name="type">The type of the vertex attribute</param>
     * <param name="fieldName">The name of the field of the vertex type</param>
     */
    public unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, string fieldName)
    {
        VertexAttributePointer(index, count, type, (int)Marshal.OffsetOf<TVertexType>(fieldName));
    }

    public unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, int offset)
    {
        _gl.VertexAttribPointer(index, count, type, false, (uint)sizeof(TVertexType), (void*)offset);
        _gl.EnableVertexAttribArray(index);
    }

    public unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, int stride, int offset)
    {
        _gl.EnableVertexAttribArray(index);
        _gl.VertexAttribPointer(index, count, type, false, (uint)(stride * sizeof(float)), (void*)(offset * sizeof(float)));
    }

    /**
     * <summary>Bind a vertex attribute to a field of the vertex type
     * only the integer types are accepted
     * </summary>
     * <param name="index">The index of the vertex attribute</param>
     * <param name="count">The number of components of the vertex attribute</param>
     * <param name="type">The type of the vertex attribute</param>
     * <param name="fieldName">The name of the field of the vertex type</param>
     */
    public unsafe void VertexAttributeIPointer(uint index, int count, VertexAttribIType type, string fieldName) =>
        VertexAttributeIPointer(index, count, type, (int)Marshal.OffsetOf(typeof(TVertexType), fieldName));

    public unsafe void VertexAttributeIPointer(uint index, int count, VertexAttribIType type, int offset)
    {
        _gl.VertexAttribIPointer(index, count, type, (uint)sizeof(TVertexType), (void*)(offset));
        _gl.EnableVertexAttribArray(index);
    }

    public void Bind()
    {
        _gl.BindVertexArray(_handle);
    }

    ~AVertexArrayObject()
    {
        Dispose(false);
    }

    protected void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _gl.DeleteVertexArray(_handle);
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetOffset<TF>(ref TVertexType vertex, ref TF field)
    {
        return Unsafe.ByteOffset(ref Unsafe.As<TVertexType, byte>(ref vertex), ref Unsafe.As<TF, byte>(ref field)).ToInt32();
    }
}
