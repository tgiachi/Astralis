using System.Collections.Immutable;
using System.Numerics;
using Arch.Core.Extensions;
using Astralis.Game.Client.Core.Buffer;
using Astralis.Game.Client.Core.Utils;
using Astralis.Game.Client.Ecs.Components;
using Astralis.Game.Client.Ecs.GameObjects.Base;
using Astralis.Game.Client.Types;
using Silk.NET.OpenGL;
using Shader = Astralis.Game.Client.Core.Shaders.Shader;

namespace Astralis.Game.Client.Ecs.GameObjects;

public class Texture2dGameObject : BaseGameObject
{
    private readonly Position2dComponent _position2dComponent;
    private readonly ShaderComponent _shaderComponent;
    private readonly TextureComponent _textureComponent;
    private readonly VertexComponent _vertexComponent;



    private readonly Transform _transform;


    private static readonly uint[] _indices =
    [
        3, 1, 0,
        3, 2, 1
    ];

    private static uint[] indices =
    {
        0u, 1u, 3u,
        1u, 2u, 3u
    };


    private static float[] vertices =
    {
        //     aPosition---- aTexCoords
        0.5f, 0.5f, 0.0f, 1.0f, 1.0f,
        0.5f, -0.5f, 0.0f, 1.0f, 0.0f,
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,
        -0.5f, 0.5f, 0.0f, 0.0f, 1.0f
    };

    public unsafe Texture2dGameObject(string textureName, Transform transform)
    {
        var gl = AstralisGameInstances.OpenGlContext.Gl;
        _transform = transform;

        _position2dComponent = new Position2dComponent(0, 0);
        _shaderComponent.Shader = new Shader(
            gl,
            Path.Combine(AstralisGameInstances.AssetDirectories[AssetDirectoryType.Shaders], "2dTexture")
        );
        _textureComponent.Texture = AstralisGameInstances.TextureManagerService().GetTexture(textureName);

        _vertexComponent.Vao = gl.GenVertexArray();
        gl.BindVertexArray(_vertexComponent.Vao);

        _vertexComponent.Vbo = gl.GenBuffer();
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexComponent.Vbo);

        fixed (float* buf = vertices)
            gl.BufferData(
                BufferTargetARB.ArrayBuffer,
                (nuint)(vertices.Length * sizeof(float)),
                buf,
                BufferUsageARB.StaticDraw
            );


        // Create the EBO.
        _vertexComponent.Ebo = gl.GenBuffer();
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _vertexComponent.Ebo);

        // Upload the indices data to the EBO.
        fixed (uint* buf = indices)
            gl.BufferData(
                BufferTargetARB.ElementArrayBuffer,
                (nuint)(indices.Length * sizeof(uint)),
                buf,
                BufferUsageARB.StaticDraw
            );


        const uint positionLoc = 0;
        gl.EnableVertexAttribArray(positionLoc);
        gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)0);

        const uint texCoordLoc = 1;
        gl.EnableVertexAttribArray(texCoordLoc);
        gl.VertexAttribPointer(
            texCoordLoc,
            2,
            VertexAttribPointerType.Float,
            false,
            5 * sizeof(float),
            (void*)(3 * sizeof(float))
        );

        gl.BindVertexArray(0);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

        gl.ActiveTexture(TextureUnit.Texture0);
        gl.BindTexture(TextureTarget.Texture2D, _textureComponent.Texture.Handle);


        // gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
        // gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
        // gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
        // gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);

        gl.BindTexture(TextureTarget.Texture2D, 0);

        int location = gl.GetUniformLocation(_shaderComponent.Shader.Handle, "uTexture");
        gl.Uniform1(location, 0);

        gl.Enable(EnableCap.Blend);
        gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        gl.Uniform1(location, 0);

        gl.Enable(EnableCap.Blend);
        gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        GLUtility.CheckError(gl);

    }


    protected override void AddComponents()
    {
        Entity.Add(_position2dComponent, _shaderComponent, _textureComponent, _vertexComponent);
        base.AddComponents();
    }
}
