using Astralis.Game.Client.Core.Buffer;

namespace Astralis.Game.Client.Ecs.Components;

public struct VertexComponent
{

    public uint Vao { get; set; }
    public uint Vbo { get; set; }
    public uint Ebo { get; set; }

    //
    // public VertexComponent(
    //     AVertexArrayObject<TextureVertex, uint> vao, ABufferObject<TextureVertex> vbo, ABufferObject<uint> ebo
    // )
    // {
    //     Vao = vao;
    //     Vbo = vbo;
    //     Ebo = ebo;
    // }
}
