using System.Numerics;
using Astralis.Core.Numerics;
using Astralis.Game.Client.Core.Visuals;

namespace Astralis.Game.Client.Core.Collision;

public class Frustum
{
    public Plane topFace { get; private set; }
    public Plane bottomFace { get; private set; }

    public Plane rightFace { get; private set; }
    public Plane leftFace { get; private set; }

    public Plane farFace { get; private set; }
    public Plane nearFace { get; private set; }

    public Frustum(Camera cam)
    {
        Update(cam);
    }

    public void Update(Camera cam)
    {
        float fovY = MathHelper.DegreesToRadians(cam.Zoom);
        float halfVSide = cam.FarDistance * MathF.Tan(fovY * .5f);
        float halfHSide = halfVSide * cam.AspectRatio;
        var frontMultFar = cam.FarDistance * cam.Front;

        nearFace = new Plane(cam.Front, cam.Position + cam.NearDistance * cam.Front);
        farFace = new Plane(-cam.Front, cam.Position + frontMultFar);
        rightFace = new Plane(Vector3.Cross(frontMultFar - cam.Right * halfHSide, cam.Up), cam.Position);
        leftFace = new Plane(Vector3.Cross(cam.Up, frontMultFar + cam.Right * halfHSide), cam.Position);
        topFace = new Plane(Vector3.Cross(cam.Right, frontMultFar - cam.Up * halfVSide), cam.Position);
        bottomFace = new Plane(Vector3.Cross(frontMultFar + cam.Up * halfVSide, cam.Right), cam.Position);
    }
}
