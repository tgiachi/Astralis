using System.Numerics;
using Astralis.Game.Client.Core.Utils;
using Serilog;
using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Core.Shaders;

public class Shader : IDisposable
{
    public uint Handle { get; }
    private readonly GL _gl;
    private bool disposed = false;
    private readonly Dictionary<string, int> _uniformLocations = new();

    public Shader(GL gl, string shaderPath)
    {
        _gl = gl;

        var vertexPath = Path.Combine(shaderPath, "VertexShader.glsl");
        var fragmentPath = Path.Combine(shaderPath, "FragmentShader.glsl");

        uint vertex = LoadShader(ShaderType.VertexShader, vertexPath);
        uint fragment = LoadShader(ShaderType.FragmentShader, fragmentPath);
        Handle = _gl.CreateProgram();
        _gl.AttachShader(Handle, vertex);
        _gl.AttachShader(Handle, fragment);
        _gl.LinkProgram(Handle);
        _gl.GetProgram(Handle, GLEnum.LinkStatus, out var status);
        if (status == 0)
        {
            throw new Exception($"Program failed to link with error: {this._gl.GetProgramInfoLog(Handle)}");
        }

        _gl.DetachShader(Handle, vertex);
        _gl.DetachShader(Handle, fragment);
        _gl.DeleteShader(vertex);
        _gl.DeleteShader(fragment);

        GLUtility.CheckError(_gl);

        _gl.GetProgram(Handle, GLEnum.ActiveUniforms, out var uniformCount);
        Log.ForContext<Shader>().Debug("{ShaderHandle} Active uniforms: {Count}", Handle, uniformCount);

        for (uint i = 0; i < uniformCount; i++)
        {
            // get the name of this uniform,
            var key = _gl.GetActiveUniform(Handle, i, out _, out _);

            Log.ForContext<Shader>().Debug("{ShaderHandle} Uniform name: {Name}", Handle, key);

            // get the location,
            var location = _gl.GetUniformLocation(Handle, key);

            // and then add it to the dictionary.
            _uniformLocations.Add(key, location);
        }
    }

    public void DebugUniforms()
    {
        _gl.GetProgram(Handle, GLEnum.ActiveUniforms, out var uniformCount);
        Log.ForContext<Shader>().Information("Active uniforms: {Count}", uniformCount);

        for (uint i = 0; i < uniformCount; i++)
        {
            string name = _gl.GetActiveUniform(Handle, i, out _, out _);
            int location = _gl.GetUniformLocation(Handle, name);
            Log.ForContext<Shader>().Information("Uniform '{Name}' at location {Location}", name, location);
        }
    }

    public void Use()
    {
        //  DebugUniforms();
        _gl.UseProgram(Handle);
    }

    public uint GetUniformBlockIndex(string name)
    {
        return _gl.GetUniformBlockIndex(Handle, name);
    }

    public void SetUniform(string name, int value)
    {
        _uniformLocations.TryGetValue(name, out int location);
        if (location == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }

        _gl.UseProgram(Handle);

        _gl.Uniform1(location, value);
        GLUtility.CheckError(_gl);
    }

    public void SetUniform(string name, float value)
    {
        _uniformLocations.TryGetValue(name, out int location);
        if (location == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }

        _gl.Uniform1(location, value);
        GLUtility.CheckError(_gl);
    }

    public unsafe void SetUniform(string name, Vector4 value)
    {
        _uniformLocations.TryGetValue(name, out int location);
        if (location == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }

        _gl.Uniform4(location, value.X, value.Y, value.Z, value.W);
        GLUtility.CheckError(_gl);
    }

    public unsafe void SetUniform(string name, Matrix4x4 value)
    {
        _uniformLocations.TryGetValue(name, out int location);
        if (location == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }

        _gl.UniformMatrix4(location, 1, false, (float*)&value);
        GLUtility.CheckError(_gl);
    }

    public int GetUniformLocation(string name)
    {
        if (!_uniformLocations.TryGetValue(name, out int value))
        {
            int location = _gl.GetUniformLocation(Handle, name);
            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }

            value = _gl.GetUniformLocation(Handle, name);
            _uniformLocations.Add(name, value);
        }

        return value;
    }

    public int GetAttribLocation(string attribName)
    {
        var result = _gl.GetAttribLocation(Handle, attribName);

        GLUtility.CheckError(_gl);
        return result;
    }


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
                _gl.DeleteProgram(Handle);
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
