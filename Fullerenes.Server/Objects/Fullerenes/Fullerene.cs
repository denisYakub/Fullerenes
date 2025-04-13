using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Fullerenes.Server.Extensions;
using Fullerenes.Server.Geometry;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.LimitedAreas;
using MessagePack;

namespace Fullerenes.Server.Objects.Fullerenes
{
    [MessagePackObject]
    [Union(0, typeof(IcosahedronFullerene))]
    public abstract class Fullerene(
        float x, float y, float z,
        float praecessioAngle, float nutatioAngle, float properRotationAngle, 
        float size)
    {
        [Key(0)]
        public float Size { get; init; } = size;
        [Key(1)]
        public Vector3 Center { get; init; } = new(x, y, z);
        [Key(3)]
        public abstract ICollection<Vector3> Vertices { get; }
        [IgnoreMember]
        public abstract IReadOnlyCollection<int[]> Faces { get; }
        [Key(4)]
        public EulerAngles EulerAngles { get; init; } = new(praecessioAngle, nutatioAngle, properRotationAngle);
        public abstract bool PartInside(Parallelepiped parallelepiped);
        public abstract bool Inside(Parallelepiped parallelepiped);
        public abstract float GenerateOuterSphereRadius();
        public abstract float GenerateInnerSphereRadius();
        public abstract float GenerateVolume();
        public abstract float GetEdgeSize();

        public virtual bool Intersect(Fullerene fullerene)
        {
            ArgumentNullException.ThrowIfNull(fullerene);

            var outerRadiusF1 = GenerateOuterSphereRadius();
            var outerRadiusF2 = fullerene.GenerateOuterSphereRadius();

            if (FiguresCollision.SpheresInside(Center, outerRadiusF1, fullerene.Center, outerRadiusF2))
                return true;

            var innerRadiusF1 = GenerateInnerSphereRadius();
            var innerRadiusF2 = fullerene.GenerateInnerSphereRadius();

            if (FiguresCollision.SpheresInside(Center, innerRadiusF1, fullerene.Center, outerRadiusF2))
                return true;

            if (FiguresCollision.SpheresInside(Center, outerRadiusF1, fullerene.Center, innerRadiusF2))
                return true;

            if (FiguresCollision.SpheresIntersect(Center, innerRadiusF1, fullerene.Center, innerRadiusF2))
                return true;

            if (!FiguresCollision.SpheresIntersect(Center, outerRadiusF1, fullerene.Center, outerRadiusF2))
                return false;

            return 
                Vertices
                .AddMidPoints(Faces)
                .Any(fullerene.Contains);
        }

        public virtual bool Contains(Vector3 point)
        {
            if (FiguresCollision.SpheresInside(point, 0, Center, GenerateInnerSphereRadius()))
                return true;

            if (!FiguresCollision.SpheresInside(point, 0, Center, GenerateOuterSphereRadius()))
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
