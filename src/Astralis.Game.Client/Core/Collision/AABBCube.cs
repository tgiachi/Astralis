using System.Numerics;
using Plane = Astralis.Game.Client.Core.Collision.Plane;

namespace Astralis.Game.Client.Core.Collision;

public class AABBCube : Volume
{
    private readonly Vector3[] _bounds = new Vector3[2];
    public Vector3 center;
    public Vector3 extents;

    public AABBCube(Vector3 min, Vector3 max)
    {
        _bounds[0] = min;
        _bounds[1] = max;
        center = (min + max) / 2;
        extents = max - center;
    }



    public bool IsInFrustrum(Frustum frustum) {
        return (IsOnOrForwardPlane(frustum.leftFace) &&
                IsOnOrForwardPlane(frustum.rightFace) &&
                IsOnOrForwardPlane(frustum.topFace) &&
                IsOnOrForwardPlane(frustum.bottomFace) &&
                IsOnOrForwardPlane(frustum.nearFace) &&
                IsOnOrForwardPlane(frustum.farFace));
    }

    private bool IsOnOrForwardPlane(Plane plane)
    {
        // Compute the projection interval radius of b onto L(t) = b.c + t * p.n
        float r = Vector3.Dot(extents, Vector3.Abs(plane.Normal));
        float s = plane.GetSignedDistanceToPlane(center);
        return -r <= s ;
    }





    public bool Intersect(Ray r, float t)
    {
        float tmin, tmax, tymin, tymax, tzmin, tzmax;

        tmin = (_bounds[r.sign[0]].X - r.orig.X) * r.invdir.X;
        tmax = (_bounds[1 - r.sign[0]].X - r.orig.X) * r.invdir.X;
        tymin = (_bounds[r.sign[1]].Y - r.orig.Y) * r.invdir.Y;
        tymax = (_bounds[1 - r.sign[1]].Y - r.orig.Y) * r.invdir.Y;

        if ((tmin > tymax) || (tymin > tmax))
            return false;

        if (tymin > tmin)
            tmin = tymin;
        if (tymax < tmax)
            tmax = tymax;

        tzmin = (_bounds[r.sign[2]].Z - r.orig.Z) * r.invdir.Z;
        tzmax = (_bounds[1 - r.sign[2]].Z - r.orig.Z) * r.invdir.Z;

        if ((tmin > tzmax) || (tzmin > tmax))
            return false;

        if (tzmin > tmin)
            tmin = tzmin;
        if (tzmax < tmax)
            tmax = tzmax;

        t = tmin;

        if (t < 0) {
            t = tmax;
            if (t < 0) return false;
        }

        return true;
    }

    public HitInfo Intersect(Ray r)
    {
        float tmin, tmax, tymin, tymax, tzmin, tzmax;

        tmin = (_bounds[r.sign[0]].X - r.orig.X) * r.invdir.X;
        tmax = (_bounds[1 - r.sign[0]].X - r.orig.X) * r.invdir.X;
        tymin = (_bounds[r.sign[1]].Y - r.orig.Y) * r.invdir.Y;
        tymax = (_bounds[1 - r.sign[1]].Y - r.orig.Y) * r.invdir.Y;

        if ((tmin > tymax) || (tymin > tmax))
            return new HitInfo(false, 0);

        if (tymin > tmin)
            tmin = tymin;
        if (tymax < tmax)
            tmax = tymax;

        tzmin = (_bounds[r.sign[2]].Z - r.orig.Z) * r.invdir.Z;
        tzmax = (_bounds[1 - r.sign[2]].Z - r.orig.Z) * r.invdir.Z;

        if ((tmin > tzmax) || (tzmin > tmax))
            return new HitInfo(false, 0);

        if (tzmin > tmin)
            tmin = tzmin;
        if (tzmax < tmax)
            tmax = tzmax;

        float t = tmin;

        if (t < 0) {
            t = tmax;
            if (t < 0) return new HitInfo(false, 0);
        }

        return new HitInfo(true,t);
    }
}
