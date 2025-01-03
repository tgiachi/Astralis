using System.Numerics;
using Arch.Core;
using Arch.System;
using Astralis.Core.Numerics;
using Astralis.Game.Client.Core.Utils;
using Astralis.Game.Client.Ecs.Components;
using Astralis.Game.Client.Types;
using ImGuiNET;
using Serilog;
using Silk.NET.OpenGL;
using Shader = Astralis.Game.Client.Core.Shaders.Shader;


namespace Astralis.Game.Client.Systems;

public class QuadRenderSystem : BaseSystem<World, GL>
{
    private readonly ILogger _logger = Log.ForContext<QuadRenderSystem>();
    private readonly Shader _shader;

    private Vector3 _color = new Vector3(1.0f, 0.5f, 0.31f);

    private float _lastTime;

    private readonly float[] _vertices =
    {
        // Position         Texture coordinates
        0.5f, 0.5f, 0.0f,  // top right
        0.5f, -0.5f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f,
        -0.5f, 0.5f, 0.0f,
    };

    private readonly uint[] _indices =
    {
        0, 1, 3,
        1, 2, 3
    };

    private uint _elementBufferObject;

    private uint _vertexBufferObject;

    private uint _vertexArrayObject;


    private readonly QueryDescription _desc = new QueryDescription()
        .WithAll<TextureComponent, Position3dComponent, QuadComponent>();

    public unsafe QuadRenderSystem(GL gl, Shader shader, World world) : base(world)
    {
        _shader = shader;

        _vertexArrayObject = gl.GenVertexArray();
        gl.BindVertexArray(_vertexArrayObject);

        _vertexBufferObject = gl.GenBuffer();
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBufferObject);


        fixed (void* v = &_vertices[0])
        {
            gl.BufferData(
                BufferTargetARB.ArrayBuffer,
                (nuint)(_vertices.Length * sizeof(float)),
                v,
                BufferUsageARB.StaticDraw
            );
        }

        _elementBufferObject = gl.GenBuffer();
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _elementBufferObject);


        fixed (void* i = &_indices[0])
        {
            gl.BufferData(
                BufferTargetARB.ElementArrayBuffer,
                (nuint)(_indices.Length * sizeof(uint)),
                i,
                BufferUsageARB.StaticDraw
            );
        }


        _shader.Use();

        var vertexLocation = (uint)_shader.GetAttribLocation("aPosition");
        gl.EnableVertexAttribArray(vertexLocation);
        gl.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        // var texCoordLocation = (uint)_shader.GetAttribLocation("aTexCoord");
        // gl.EnableVertexAttribArray(texCoordLocation);
        // gl.VertexAttribPointer(
        //     texCoordLocation,
        //     2,
        //     VertexAttribPointerType.Float,
        //     false,
        //     5 * sizeof(float),
        //     3 * sizeof(float)
        // );
    }

    public override unsafe void Update(in GL t)
    {
        var gl = t;

        ImGui.Begin("Color chooser");
        ImGui.ColorEdit3("Triangle color", ref _color);
        ImGui.End();

        _lastTime += (float)ImGui.GetTime();

        gl.BindVertexArray(_vertexArrayObject);
        World.Query(
            in _desc,
            (ref TextureComponent texture, ref Position3dComponent position, ref QuadComponent quad) =>
            {
                if (AstralisGameInstances.Camera == null)
                {
                    return;
                }

                var camera = AstralisGameInstances.Camera;

                _shader.Use();

                _shader.SetUniform("uColor", new Vector4(_color.X, _color.Y, _color.Z, 1.0f));

                gl.BindVertexArray(_vertexArrayObject);

                var model = Matrix4x4.Identity * Matrix4x4.CreateRotationX((float)MathHelper.DegreesToRadians(_lastTime)) *
                            Matrix4x4.CreateTranslation(position.Position);
                // Rotate(ref model, quad.Face);
                _shader.SetUniform("model", model);
                _shader.SetUniform("view", camera.GetViewMatrix());
                _shader.SetUniform("projection", camera.GetProjectionMatrix());

                gl.DrawElements(PrimitiveType.Triangles, (uint)_indices.Length, DrawElementsType.UnsignedInt, (void*)null);
            }
        );
    }

    private static void Rotate(ref Matrix4x4 model, BlockFace face)
    {
        switch (face)
        {
            case BlockFace.FRONT:
                break;
            case BlockFace.BACK:
                model = Matrix4x4.CreateRotationY(MathF.PI) * model;
                break;
            case BlockFace.LEFT:
                model = Matrix4x4.CreateRotationY(-MathF.PI / 2) * model;
                break;
            case BlockFace.RIGHT:
                model = Matrix4x4.CreateRotationY(MathF.PI / 2) * model;
                break;
            case BlockFace.TOP:
                model = Matrix4x4.CreateRotationX(-MathF.PI / 2) * model;
                break;
            case BlockFace.BOTTOM:
                model = Matrix4x4.CreateRotationX(MathF.PI / 2) * model;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(face), face, null);
        }
    }
}
