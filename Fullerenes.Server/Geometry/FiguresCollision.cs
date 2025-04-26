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
        /// <param name="center1">Vector3 center of the first shpere</param>
        /// <param name="r1">float radius of the first shpere</param>
        /// <param name="center2">Vector3 center of the second shpere</param>
        /// <param name="r2">float radius of the second shpere</param>
        /// <returns>true if contains, elase false</returns>
        public static bool SpheresInside(
            Vector3 center1, float r1,
            Vector3 center2, float r2
        )
        {
            ArgumentNullException.ThrowIfNull(center1);
            ArgumentNullException.ThrowIfNull(center2);

            var distance = Vector3.Distance(center1, center2);

            return distance + r1 <= r2 || distance + r2 <= r2;
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
            Vector3 center1, float r1,
            Vector3 center2, float r2
        )
        {
            ArgumentNullException.ThrowIfNull(center1);
            ArgumentNullException.ThrowIfNull(center2);

            return Vector3.Distance(center2, center1) < r1 + r2;
        }
        /// <summary>
        /// Checks if point is inside parallelepiped
        /// </summary>
        /// <param name="parallelepiped"></param>
        /// <param name="point"></param>
        /// <returns>true if point inside</returns>
        public static bool Pointinside(Cube parallelepiped, Vector3 point)
        {
            ArgumentNullException.ThrowIfNull(parallelepiped);

            return
                MathF.Abs(point.X - parallelepiped.Center.X) <= parallelepiped.Length / 2 &&
                MathF.Abs(point.Y - parallelepiped.Center.Y) <= parallelepiped.Width / 2 &&
                MathF.Abs(point.Z - parallelepiped.Center.Z) <= parallelepiped.Height / 2;
        }
        /// <summary>
        /// Checks two figures intersection using Separating Axis Theorem
        /// </summary>
        /// <param name="vertices1">Vertices of the first figure</param>
        /// <param name="faces1">Faces of the first figure</param>
        /// <param name="vertices2">Vertices of the second figure</param>
        /// <param name="faces2">Faces of the second figure</param>
        /// <returns>true if intersect, else false</returns>
        public static bool FiguresIntersectUsingSat(
            ICollection<Vector3> vertices1, ICollection<int[]> faces1,
            ICollection<Vector3> vertices2, ICollection<int[]> faces2)
        {
            ArgumentNullException.ThrowIfNull(faces1);
            ArgumentNullException.ThrowIfNull(faces2);

            foreach (var face in faces1)
            {
                Vector3 normal = Formulas.GetNormal(vertices1.ElementAt(face[0]), vertices1.ElementAt(face[1]), vertices1.ElementAt(face[2]));

                if (Formulas.IsSeparatingAxis(normal, vertices1, vertices2))
                    return false;
            }

            foreach (var face in faces2)
            {
                Vector3 normal = Formulas.GetNormal(vertices2.ElementAt(face[0]), vertices2.ElementAt(face[1]), vertices2.ElementAt(face[2]));
                if (Formulas.IsSeparatingAxis(normal, vertices1, vertices2))
                    return false;
            }

            return true;
        }
        /// <summary>
        /// Checks if sphere intersect with parallelepiped
        /// </summary>
        /// <param name="area">Parallelepiped</param>
        /// <param name="sphereCenter">Center of th sphere</param>
        /// <param name="sphereRadius">Radius of the sphere</param>
        /// <returns>true if intersect, else false</returns>
        public static bool Intersects(Cube area, Vector3 sphereCenter, float sphereRadius)
        {
            ArgumentNullException.ThrowIfNull(area);

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
