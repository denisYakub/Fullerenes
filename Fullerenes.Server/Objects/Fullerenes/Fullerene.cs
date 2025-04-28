using System.Numerics;
using Fullerenes.Server.Extensions;
using Fullerenes.Server.Geometry;
using Fullerenes.Server.Objects.CustomStructures;

namespace Fullerenes.Server.Objects.Fullerenes
{
    public abstract class Fullerene(
        float x, float y, float z,
        float praecessioAngle, float nutatioAngle, float properRotationAngle, 
        float size)
    {
        public float Size => size;
        public Vector3 Center => new(x, y, z);
        public abstract ICollection<Vector3> Vertices { get; }
        public abstract IReadOnlyCollection<int[]> Faces { get; }
        public EulerAngles EulerAngles => new(praecessioAngle, nutatioAngle, properRotationAngle);

        public abstract float OuterSphereRadius { get; }
        public abstract float InnerSphereRadius { get; }
        public abstract float GenerateVolume();
        public abstract float GetEdgeSize();

        public virtual bool Intersect(Fullerene fullerene)
        {
            ArgumentNullException.ThrowIfNull(fullerene);

            var outerRadiusF1 = OuterSphereRadius;
            var centerF1 = Center;

            var outerRadiusF2 = fullerene.OuterSphereRadius;
            var centerF2 = fullerene.Center;

            if (FiguresCollision.SpheresInside(in centerF1, outerRadiusF1, in centerF2, outerRadiusF2))
                return true;

            var innerRadiusF1 = InnerSphereRadius;
            var innerRadiusF2 = fullerene.InnerSphereRadius;

            if (FiguresCollision.SpheresInside(in centerF1, innerRadiusF1, in centerF2, outerRadiusF2))
                return true;

            if (FiguresCollision.SpheresInside(in centerF1, outerRadiusF1, in centerF2, innerRadiusF2))
                return true;

            if (FiguresCollision.SpheresIntersect(in centerF1, innerRadiusF1, in centerF2, innerRadiusF2))
                return true;

            if (!FiguresCollision.SpheresIntersect(in centerF1, outerRadiusF1, in centerF2, outerRadiusF2))
                return false;

            if (Vertices.Count != 42)
                throw new Exception();

            return
                Vertices
                .Any(fullerene.Contains);
        }

        public virtual bool Contains(Vector3 point)
        {
            var centerF = Center;

            if (FiguresCollision.SpheresInside(in point, 0, in centerF, InnerSphereRadius))
                return true;

            if (!FiguresCollision.SpheresInside(in point, 0, in centerF, OuterSphereRadius))
                return false;

            var vertices = Vertices;

            foreach (var face in Faces)
            {
                Vector3 a = vertices.ElementAt(face[0]);
                Vector3 b = vertices.ElementAt(face[1]);
                Vector3 c = vertices.ElementAt(face[2]);

                Vector3 normal = Vector3.Cross(b - a, c - a);

                float dotProduct = Vector3.Dot(normal, point - a);

                if (dotProduct > 0.0f)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
