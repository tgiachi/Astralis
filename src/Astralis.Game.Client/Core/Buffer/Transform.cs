using System.Numerics;
using Astralis.Core.Numerics;
using ImGuiNET;

namespace Astralis.Game.Client.Core.Buffer;

public class Transform
{
    public Vector3 Position { get; set; }
    public Vector3 Scale { get; set; }
    public Quaternion Rotation { get; set; }

    public Matrix4x4 TransformMatrix => Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateScale(Scale) *
                                        Matrix4x4.CreateTranslation(Position);

    /**
     * @brief Constructor of the Transform class
     * @param position the position of the transform (default is 0)
     * @param scale the scale of the transform (default is 1)
     * @param rotation the rotation of the transform (default is 0)
     */
    public Transform(Vector3 position = default, Vector3? scale = null, Quaternion rotation = default)
    {
        Position = position;
        if (scale is not null)
        {
            Scale = (Vector3)scale;
        }
        else
        {
            Scale = Vector3.One;
        }

        Rotation = rotation;
    }
}
