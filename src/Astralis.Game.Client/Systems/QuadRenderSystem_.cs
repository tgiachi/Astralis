/*
using System.Numerics;
using Arch.Core;
using Arch.System;
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


    Vector3 _color = new(0.5f, 0.5f, 0.0f);

    // Create the vertices for our triangle. These are listed in normalized device coordinates (NDC)
    // In NDC, (0, 0) is the center of the screen.
    // Negative X coordinates move to the left, positive X move to the right.
    // Negative Y coordinates move to the bottom, positive Y move to the top.
    // OpenGL only supports rendering in 3D, so to create a flat triangle, the Z coordinate will be kept as 0.
    private readonly float[] _vertices =
    {
        -0.5f, -0.5f, 0.0f, // Bottom-left vertex
        0.5f, -0.5f, 0.0f,  // Bottom-right vertex
        0.0f, 0.5f, 0.0f    // Top vertex
    };

    private uint _vertexBufferObject;

    private uint _vertexArrayObject;


    private readonly QueryDescription _desc = new QueryDescription()
        .WithAll<TextureComponent, Position3dComponent, QuadComponent>();

    public unsafe QuadRenderSystem(GL gl, Shader shader, World world) : base(world)
    {
        _shader = shader;

        _vertexBufferObject = gl.GenBuffer();

        // Now, bind the buffer. OpenGL uses one global state, so after calling this,
        // all future calls that modify the VBO will be applied to this buffer until another buffer is bound instead.
        // The first argument is an enum, specifying what type of buffer we're binding. A VBO is an ArrayBuffer.
        // There are multiple types of buffers, but for now, only the VBO is necessary.
        // The second argument is the handle to our buffer.
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBufferObject);

        // Finally, upload the vertices to the buffer.
        // Arguments:
        //   Which buffer the data should be sent to.
        //   How much data is being sent, in bytes. You can generally set this to the length of your array, multiplied by sizeof(array type).
        //   The vertices themselves.
        //   How the buffer will be used, so that OpenGL can write the data to the proper memory space on the GPU.
        //   There are three different BufferUsageHints for drawing:
        //     StaticDraw: This buffer will rarely, if ever, update after being initially uploaded.
        //     DynamicDraw: This buffer will change frequently after being initially uploaded.
        //     StreamDraw: This buffer will change on every frame.
        //   Writing to the proper memory space is important! Generally, you'll only want StaticDraw,
        //   but be sure to use the right one for your use case.
        fixed (void* v = &_vertices[0])
        {
            gl.BufferData(
                BufferTargetARB.ArrayBuffer,
                (nuint)(_vertices.Length * sizeof(float)),
                v,
                BufferUsageARB.StaticDraw
            );
        }

        // One notable thing about the buffer we just loaded data into is that it doesn't have any structure to it. It's just a bunch of floats (which are actaully just bytes).
        // The opengl driver doesn't know how this data should be interpreted or how it should be divided up into vertices. To do this opengl introduces the idea of a
        // Vertex Array Obejct (VAO) which has the job of keeping track of what parts or what buffers correspond to what data. In this example we want to set our VAO up so that
        // it tells opengl that we want to interpret 12 bytes as 3 floats and divide the buffer into vertices using that.
        // To do this we generate and bind a VAO (which looks deceptivly similar to creating and binding a VBO, but they are different!).
        _vertexArrayObject = gl.GenVertexArray();
        gl.BindVertexArray(_vertexArrayObject);

        // Now, we need to setup how the vertex shader will interpret the VBO data; you can send almost any C datatype (and a few non-C ones too) to it.
        // While this makes them incredibly flexible, it means we have to specify how that data will be mapped to the shader's input variables.

        // To do this, we use the GL.VertexAttribPointer function
        // This function has two jobs, to tell opengl about the format of the data, but also to associate the current array buffer with the VAO.
        // This means that after this call, we have setup this attribute to source data from the current array buffer and interpret it in the way we specified.
        // Arguments:
        //   Location of the input variable in the shader. the layout(location = 0) line in the vertex shader explicitly sets it to 0.
        //   How many elements will be sent to the variable. In this case, 3 floats for every vertex.
        //   The data type of the elements set, in this case float.
        //   Whether or not the data should be converted to normalized device coordinates. In this case, false, because that's already done.
        //   The stride; this is how many bytes are between the last element of one vertex and the first element of the next. 3 * sizeof(float) in this case.
        //   The offset; this is how many bytes it should skip to find the first element of the first vertex. 0 as of right now.
        // Stride and Offset are just sort of glossed over for now, but when we get into texture coordinates they'll be shown in better detail.
        gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

        // Enable variable 0 in the shader.
        gl.EnableVertexAttribArray(0);

        _shader.Use();
    }

    public override unsafe void Update(in GL t)
    {
        var gl = t;

        _shader.Use();


        ImGui.Begin("Color chooser");
        ImGui.ColorEdit3("Triangle color", ref _color);
        ImGui.End();



        World.Query(
            in _desc,
            (ref TextureComponent texture, ref Position3dComponent position, ref QuadComponent quad) =>
            {
                if (AstralisGameInstances.Camera == null)
                {
                    return;
                }

                _shader.SetUniform("ourColor", new Vector4(_color.X, _color.Y, _color.Z, 1.0f));

                gl.BindVertexArray(_vertexArrayObject);

                gl.DrawArrays(PrimitiveType.Triangles, 0, 3);

                GLUtility.CheckError(gl);
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
*/
