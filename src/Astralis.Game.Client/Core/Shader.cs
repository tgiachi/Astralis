using System.Numerics;
using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Core;

public class Shader : IDisposable
{
    private readonly uint handle;
    private readonly GL _gl;
    private bool disposed = false;
    private readonly Dictionary<string, int> _uniformLocations = new();

    public Shader(GL gl, string vertexPath, string fragmentPath)
    {
        _gl = gl;

        uint vertex = LoadShader(ShaderType.VertexShader, vertexPath);
        uint fragment = LoadShader(ShaderType.FragmentShader, fragmentPath);
        handle = _gl.CreateProgram();
        _gl.AttachShader(handle, vertex);
        _gl.AttachShader(handle, fragment);
        _gl.LinkProgram(handle);
        _gl.GetProgram(handle, GLEnum.LinkStatus, out var status);
        if (status == 0)
        {
            throw new Exception($"Program failed to link with error: {this._gl.GetProgramInfoLog(handle)}");
        }

        _gl.DetachShader(handle, vertex);
        _gl.DetachShader(handle, fragment);
        _gl.DeleteShader(vertex);
        _gl.DeleteShader(fragment);
    }

    public void Use()
    {
        _gl.UseProgram(handle);
    }

    public uint GetUniformBlockIndex(string name)
    {
        return _gl.GetUniformBlockIndex(handle, name);
    }

    public void SetUniform(string name, int value) => _gl.Uniform1(GetUniformLocation(name), value);

    public int GetUniformLocation(string name)
    {
        if (!_uniformLocations.ContainsKey(name))
        {
            int location = _gl.GetUniformLocation(handle, name);
            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }

            _uniformLocations.Add(name, _gl.GetUniformLocation(handle, name));
        }

        return _uniformLocations[name];
    }

    public unsafe void SetUniform(string name, Matrix4x4 value) =>
        _gl.UniformMatrix4(GetUniformLocation(name), 1, false, (float*)&value);

    public void SetUniform(string name, float value) => _gl.Uniform1(GetUniformLocation(name), value);

    public void SetUniform(string name, Vector3 value) => _gl.Uniform3(GetUniformLocation(name), value.X, value.Y, value.Z);

    ~Shader()
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
                _gl.DeleteProgram(handle);
            }

            disposed = true;
        }
    }

    private uint LoadShader(ShaderType type, string path)
    {
        string src = File.ReadAllText(path);
        uint hdl = _gl.CreateShader(type);
        _gl.ShaderSource(hdl, src);
        _gl.CompileShader(hdl);
        string infoLog = _gl.GetShaderInfoLog(hdl);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
        }

        return hdl;
    }
}
