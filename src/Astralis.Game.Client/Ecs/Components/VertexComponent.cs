using Astralis.Game.Client.Core.Buffer;

namespace Astralis.Game.Client.Ecs.Components;

public record struct VertexComponent
{
    public AVertexArrayObject<TextureVertex, uint> Vao { get; set; }

    public BufferObject<TextureVertex> Vbo { get; set; }
    public BufferObject<uint> Ebo { get; set; }

    public VertexComponent(
        AVertexArrayObject<TextureVertex, uint> vao, BufferObject<TextureVertex> vbo, BufferObject<uint> ebo
    )
    {
        Vao = vao;
        Vbo = vbo;
        Ebo = ebo;
    }
}
