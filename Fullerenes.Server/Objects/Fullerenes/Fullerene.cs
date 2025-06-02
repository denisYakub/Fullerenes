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
        public float Size { get; } = size;
        public Vector3 Center { get; } = new(x, y, z);
        public EulerAngles EulerAngles { get; } = 
            new(){
                PraecessioAngle = praecessioAngle,
                NutatioAngle = nutatioAngle,
                ProperRotationAngle = properRotationAngle,
            };
        public abstract ReadOnlySpan<Vector3> Vertices { get; }
        public abstract IReadOnlyCollection<int[]> Faces { get; }

        protected readonly Matrix4x4 rotationMatrix = 
            Formulas
            .CreateRotationMatrix(praecessioAngle, nutatioAngle, properRotationAngle);

        public abstract float OuterSphereRadius { get; }
        public abstract float InnerSphereRadius { get; }
        public abstract float GenerateVolume();
        public abstract float GetEdgeSize();

        public virtual bool Intersect(Fullerene fullerene)
        {
            var centerF1 = Center;
            var centerF2 = fullerene.Center;

            var innerRadiusF1 = InnerSphereRadius;
            var innerRadiusF2 = fullerene.InnerSphereRadius;

            if (FiguresCollision.SpheresIntersect(in centerF1, innerRadiusF1, in centerF2, innerRadiusF2))
                return true;

            var outerRadiusF1 = OuterSphereRadius;
            var outerRadiusF2 = fullerene.OuterSphereRadius;

            if (!FiguresCollision.SpheresIntersect(in centerF1, OuterSphereRadius, in centerF2, outerRadiusF2))
                return false;

            foreach (var vertex in Vertices)
                if(fullerene.Contains(vertex))
                    return true;

            return false;
        }

        public virtual bool Contains(Vector3 point)
        {
            var centerF = Center;

            if (FiguresCollision.Pointinside(in centerF, InnerSphereRadius, in point))
                return true;

            if (!FiguresCollision.Pointinside(in centerF, OuterSphereRadius, in point))
                return false;

            foreach (var face in Faces)
            {
                Vector3 a = Vertices[face[0]];
                Vector3 b = Vertices[face[1]];
                Vector3 c = Vertices[face[2]];

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
