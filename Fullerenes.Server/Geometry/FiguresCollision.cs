using System.Numerics;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;

namespace Fullerenes.Server.Geometry
{
    public static class FiguresCollision
    {
        /// <summary>
        /// Checks if at least one sphere is 
        /// contained within another
        /// </summary>
        /// <param name="innerC">Vector3 center of the inner shpere</param>
        /// <param name="r1">float radius of the inner shpere</param>
        /// <param name="center2">Vector3 center of the outer shpere</param>
        /// <param name="outerR">float radius of the outer shpere</param>
        /// <returns>true if contains, else false</returns>
        public static bool SpheresInside(
            in Vector3 innerC, float innerR,
            in Vector3 outerC, float outerR
        )
        {
            var distanceSquar = Vector3.DistanceSquared(innerC, outerC);
            float radiusDiff = outerR - innerR;

            return radiusDiff >= 0 && distanceSquar <= radiusDiff * radiusDiff;
        }
        /// <summary>
        /// Checks two spheres intersection
        /// </summary>
        /// <param name="center1">Center of the first shpere</param>
        /// <param name="r1">Radius of the first shpere</param>
        /// <param name="center2">Center of the second shpere</param>
        /// <param name="r2">Radius of the second shpere</param>
        /// <returns>true if intersect, else false</returns>
        public static bool SpheresIntersect(
            in Vector3 center1, float r1,
            in Vector3 center2, float r2
        )
        {
            return Vector3.Distance(center2, center1) < (r1 + r2) * (r1 + r2);
        }
        /// <summary>
        /// Checks if point is inside parallelepiped
        /// </summary>
        /// <param name="parallelepiped"></param>
        /// <param name="point"></param>
        /// <returns>true if point inside</returns>
        public static bool Pointinside(Cube parallelepiped, in Vector3 point)
        {
           return
                MathF.Abs(point.X - parallelepiped.Center.X) <= parallelepiped.Length / 2 &&
                MathF.Abs(point.Y - parallelepiped.Center.Y) <= parallelepiped.Width / 2 &&
                MathF.Abs(point.Z - parallelepiped.Center.Z) <= parallelepiped.Height / 2;
        }
        /// <summary>
        /// Checks if sphere intersect with parallelepiped
        /// </summary>
        /// <param name="area">Parallelepiped</param>
        /// <param name="sphereCenter">Center of th sphere</param>
        /// <param name="sphereRadius">Radius of the sphere</param>
        /// <returns>true if intersect, else false</returns>
        public static bool Intersects(Cube area, in Vector3 sphereCenter, float sphereRadius)
        {
            float minX = area.Center.X - area.Width / 2;
            float maxX = area.Center.X + area.Width / 2;
            float minY = area.Center.Y - area.Height / 2;
            float maxY = area.Center.Y + area.Height / 2;
            float minZ = area.Center.Z - area.Length / 2;
            float maxZ = area.Center.Z + area.Length / 2;

            float closestX = Math.Clamp(sphereCenter.X, minX, maxX);
            float closestY = Math.Clamp(sphereCenter.Y, minY, maxY);
            float closestZ = Math.Clamp(sphereCenter.Z, minZ, maxZ);

            float distanceSquared =
                (closestX - sphereCenter.X) * (closestX - sphereCenter.X) +
                (closestY - sphereCenter.Y) * (closestY - sphereCenter.Y) +
                (closestZ - sphereCenter.Z) * (closestZ - sphereCenter.Z);

            return distanceSquared <= sphereRadius * sphereRadius;
        }
    }
}
