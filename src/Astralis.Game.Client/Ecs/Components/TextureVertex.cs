using System.Numerics;

namespace Astralis.Game.Client.Ecs.Components;

public struct TextureVertex(Vector3 position, Vector2 texCoord)
{
    public Vector3 Position { get; set; } = position;
    public Vector2 TexCoord { get; set; } = texCoord;
}
