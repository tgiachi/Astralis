using System.Numerics;
using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Core.Shaders;

public class ComputeShader : IDisposable
{
    public uint Handle { get; private set; }
    private readonly GL _gl;
    private bool disposed = false;

    public ComputeShader(GL gl, string computePath)
    {
        _gl = gl;

        var compute = LoadShader(ShaderType.ComputeShader, computePath);

        Handle = gl.CreateProgram();
        gl.AttachShader(Handle, compute);
        gl.LinkProgram(Handle);
        gl.GetProgram(Handle, GLEnum.LinkStatus, out var status);
        if (status == 0)
        {
            throw new Exception($"Program failed to link with error: {gl.GetProgramInfoLog(Handle)}");
        }

        gl.DetachShader(Handle, compute);
        gl.DeleteShader(compute);
    }

    public void Use()
    {
        _gl.UseProgram(Handle);
    }

    private uint LoadShader(ShaderType type, string path)
    {
        string src = File.ReadAllText(path);
        uint handle = _gl.CreateShader(type);
        _gl.ShaderSource(handle, src);
        _gl.CompileShader(handle);
        string infoLog = _gl.GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
        }

        return handle;
    }

    public uint GetUniformBlockIndex(string name)
    {
        return _gl.GetUniformBlockIndex(Handle, name);
    }

    public void SetUniform(string name, int value)
    {
        int location = _gl.GetUniformLocation(Handle, name);
        if (location == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }

        _gl.Uniform1(location, value);
    }

    public void SetUniform(string name, float value)
    {
        int location = _gl.GetUniformLocation(Handle, name);
        if (location == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }

        _gl.Uniform1(location, value);
    }


    public void SetUniform(string name, Vector3 value)
    {
        int location = _gl.GetUniformLocation(Handle, name);
        if (location == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }

        _gl.Uniform3(location, value.X, value.Y, value.Z);
    }

    ~ComputeShader()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _gl.DeleteProgram(Handle);
            }

            disposed = true;
        }
    }
}
