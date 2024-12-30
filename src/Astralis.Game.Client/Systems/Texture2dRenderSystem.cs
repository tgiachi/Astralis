using Arch.Core;
using Arch.System;
using Astralis.Game.Client.Ecs.Components;
using Silk.NET.OpenGL;

namespace Astralis.Game.Client.Systems;

public class Texture2dRenderSystem : BaseSystem<World, GL>
{
    private readonly QueryDescription _desc = new QueryDescription()
        .WithAll<Position2dComponent, ShaderComponent, TextureComponent, VertexComponent>();

    public Texture2dRenderSystem(World world) : base(world)
    {
    }

    public override unsafe void Update(in GL t)
    {
        var gl = t;
        World.Query(
            in _desc,
            (
                ref Position2dComponent _, ref ShaderComponent shader, ref TextureComponent texture,
                ref VertexComponent vertexComponent
            ) =>
            {
                gl.BindVertexArray(vertexComponent.Vao);
                gl.UseProgram(shader.Shader.Handle);

                gl.ActiveTexture(TextureUnit.Texture0);
                gl.BindTexture(TextureTarget.Texture2D, texture.Texture.Handle);

                gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*)0);
            }
        );
        base.Update(in t);
    }
}
