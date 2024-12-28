using System.Numerics;

namespace Astralis.Game.Client.Core.Collision;

public struct Plane
{
    public Vector3 Normal { get; }
    public Vector3 Center { get; }
    private readonly float distance = 0;

    public Plane(
        Vector3 a,
        Vector3 b,
        Vector3 d,
        Vector3 center)
    {
        Normal = Vector3.Cross(Vector3.Subtract(b, a),
            Vector3.Subtract(d, a));
        Normal = Vector3.Normalize(Normal);
        Center = center;
        Center = center;
        distance = CalculateDistanceToOrigin(Normal, Center);
    }

    public Plane(Vector3 normal, Vector3 center)
    {
        Normal = normal;
        Center = center;
        distance = CalculateDistanceToOrigin(normal, center);
    }
    public static float CalculateDistanceToOrigin(Vector3 normal, Vector3 pointOnPlane)
    {
        return Vector3.Dot(normal, pointOnPlane);
    }


    public float GetSignedDistanceToPlane(Vector3 point)
    {
        return Vector3.Dot(Normal, point) - distance;
    }


    public bool Intersect(Ray ray, ref float t)
    {
        float denom = Vector3.Dot(Normal, ray.dir);
        if (denom > 1.0e-6) {
            Vector3 p010 = Vector3.Subtract(Center, ray.orig );
            t = Vector3.Dot(p010, Normal) / denom;
            return (t >= 0);
        }

        return false;
    }

    public HitInfo Intersect(Ray ray)
    {
        float denom = Vector3.Dot(Normal, ray.dir);
        if (denom > 1.0e-6) {
            Vector3 p010 = Vector3.Subtract(Center, ray.orig );
            float t = Vector3.Dot(p010, Normal) / denom;
            return new HitInfo(t >= 0, t) ;
        }

        return new HitInfo(false, 0);
    }
}
