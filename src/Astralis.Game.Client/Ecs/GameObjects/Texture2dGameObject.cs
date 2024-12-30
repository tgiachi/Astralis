using System.Collections.Immutable;
using System.Numerics;
using Arch.Core.Extensions;
using Astralis.Game.Client.Core.Buffer;
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

    private readonly Transform _transform = new();


    private static readonly uint[] _indices =
    [
        3, 1, 0,
        3, 2, 1
    ];

    private TextureVertex[] vertices = new TextureVertex[4];

    private static readonly ImmutableArray<TextureVertex> VERTICES_BASE = ImmutableArray.Create<TextureVertex>(
        new(new Vector3(1f, 1f, 0.0f), new Vector2(1.0f, 1.0f)),   // top right
        new(new Vector3(1f, -1f, 0.0f), new Vector2(1.0f, 0.0f)),  // bottom right
        new(new Vector3(-1f, -1f, 0.0f), new Vector2(0.0f, 0.0f)), // bottom left
        new(new Vector3(-1f, 1f, 0.0f), new Vector2(0.0f, 1.0f))   // top left
    );


    public Texture2dGameObject(string textureName, Vector2 position)
    {
        var gl = AstralisGameInstances.OpenGlContext.Gl;
        _position2dComponent = new Position2dComponent(position.X, position.Y);
        _shaderComponent.Shader = new Shader(
            gl,
            Path.Combine(AstralisGameInstances.AssetDirectories[AssetDirectoryType.Shaders], "2dTexture")
        );
        _textureComponent.Texture = AstralisGameInstances.TextureManagerService().GetTexture(textureName);

        _vertexComponent.Ebo = new BufferObject<uint>(gl, _indices, BufferTargetARB.ElementArrayBuffer);
        _vertexComponent.Vbo = new BufferObject<TextureVertex>(
            gl,
            4,
            BufferTargetARB.ArrayBuffer,
            BufferUsageARB.StaticDraw
        );
        _vertexComponent.Vao = new AVertexArrayObject<TextureVertex, uint>(gl, _vertexComponent.Vbo, _vertexComponent.Ebo);

        _vertexComponent.Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 0);
        _vertexComponent.Vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, "texCoord");


        UpdateData();
    }


    private void UpdateData()
    {
        VERTICES_BASE.CopyTo(vertices);
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].Position = Vector3.Transform(vertices[i].Position, _transform.TransformMatrix);
        }

        _vertexComponent.Vbo.SendData(vertices, 0);
    }


    protected override void AddComponents()
    {
        Entity.Add(_position2dComponent, _shaderComponent, _textureComponent, _vertexComponent);
        base.AddComponents();
    }
}
