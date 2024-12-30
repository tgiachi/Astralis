using System.Numerics;
using System.Runtime.InteropServices;

namespace Astralis.Game.Client.Ecs.Components;

[StructLayout(LayoutKind.Sequential)]
public struct TextureVertex
{
    public Vector3 Position;
    private Vector2 texCoord;

    public TextureVertex(Vector3 vector3, Vector2 vector2)
    {
        Position = vector3;
        texCoord = vector2;
    }
}
