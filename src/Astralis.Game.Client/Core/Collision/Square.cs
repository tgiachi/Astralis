using System.Numerics;

namespace Astralis.Game.Client.Core.Collision;

public class Square
{
    private Plane plane;

    private Vector3 point1;
    private Vector3 point2;
    private Vector3 point3;
    private Vector3 point4;
    public Square(Vector3 point1,
        Vector3 point2,
        Vector3 point3,
        Vector3 point4,
        Vector3 center)
    {
        plane = new Plane(point1, point2, point4, center);
        this.point1 = point1;
        this.point2 = point2;
        this.point3 = point3;
        this.point4 = point4;
    }

    public bool Intersect(Ray ray)
    {
        HitInfo hitInfo = plane.Intersect(ray);
        if (hitInfo.IsHit == false) return false;
        float t = hitInfo.FNorm;


        Vector3 intersectPoint = ray.orig + (t * ray.dir);
        float o1 = Orient(intersectPoint, point1, point2, plane.Normal);
        float o2 = Orient(intersectPoint, point2, point3, plane.Normal);
        float o3 = Orient(intersectPoint, point3, point4, plane.Normal);
        float o4 = Orient(intersectPoint, point4, point1, plane.Normal);
        return (o1 >= 0 && o2 >= 0 && o3 >= 0 && o4 >= 0) ||
               (o1 <= 0 && o2 <= 0 && o3 <= 0 && o4 <= 0);
    }

    public HitInfo IntersectInfo(Ray ray)
    {
        HitInfo hitInfo = plane.Intersect(ray);
        if (hitInfo.IsHit == false) return new HitInfo(false, hitInfo.FNorm) ;
        float t = hitInfo.FNorm;


        Vector3 intersectPoint = ray.orig + (t * ray.dir);
        float o1 = Orient(intersectPoint, point1, point2, plane.Normal);
        float o2 = Orient(intersectPoint, point2, point3, plane.Normal);
        float o3 = Orient(intersectPoint, point3, point4, plane.Normal);
        float o4 = Orient(intersectPoint, point4, point1, plane.Normal);
        return new HitInfo( (o1 >= 0 && o2 >= 0 && o3 >= 0 && o4 >= 0) ||
               (o1 <= 0 && o2 <= 0 && o3 <= 0 && o4 <= 0), hitInfo.FNorm);
    }

    private float Orient(Vector3 a, Vector3 b, Vector3 c, Vector3 n)
    {
        return Vector3.Dot(Vector3.Cross((b - a), (c - a)), n);
    }

}
