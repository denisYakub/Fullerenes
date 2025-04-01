using System.Numerics;
using Fullerenes.Server.Extensions;
using Fullerenes.Server.Geometry;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.LimitedAreas;

namespace Fullerenes.Server.Objects.Fullerenes
{
    public abstract class Fullerene(float x, float y, float z,
        float praecessioAngle, float nutatioAngle, float properRotationAngle, float size)
    {
        public int Id { get; set; }
        public int Series { get; set; }
        public int LimitedAreaId { get; set; }
        public LimitedArea? LimitedArea { get; set; }
        public Vector3 Center { get; init; } = new(x, y, z);
        public EulerAngles EulerAngles { get; init; } = new(praecessioAngle, nutatioAngle, properRotationAngle);
        public float Size { get; init; } = size;
        public float GenerateOuterSphereRadius() => Size;
        public abstract float GenerateInnerSphereRadius();
        public abstract ICollection<int[]> GenerateFacesIndices();
        public abstract ICollection<Vector3> GenerateStartVerticesPositions();
        public abstract bool Inside(Parallelepiped parallelepiped);
        public abstract bool PartInside(Parallelepiped parallelepiped);
        public virtual ICollection<Vector3> GenerateVerticesPositions() =>
            GenerateStartVerticesPositions()
            .Rotate(EulerAngles)
            .Shift(Center);
        public virtual bool Intersect(IReadOnlyCollection<Fullerene> fullerenes) =>
            fullerenes.Any() &&
            fullerenes.AsParallel().Any(Intersect);
        public virtual bool Intersect(Fullerene fullerene)
        {
            ArgumentNullException.ThrowIfNull(fullerene);

            var dist = Vector3.Distance(Center, fullerene.Center);

            var outerRadiusF1 = GenerateOuterSphereRadius();
            var outerRadiusF2 = fullerene.GenerateOuterSphereRadius();

            if (FigureCollision.SpheresInside(Center, outerRadiusF1, fullerene.Center, outerRadiusF2))
                return true;

            if (FigureCollision.SpheresIntersect(Center, GenerateInnerSphereRadius(), fullerene.Center, fullerene.GenerateInnerSphereRadius()))
                return true;

            if (!FigureCollision.SpheresIntersect(Center, outerRadiusF1, fullerene.Center, outerRadiusF2))
                return false;

            var dotsF1 = GenerateStartVerticesPositions();
            var innerRadiusF1 = GenerateInnerSphereRadius();

            var dotsF2 = fullerene
                .GenerateStartVerticesPositions()
                .AddMidPoints(fullerene.GenerateFacesIndices())
                .Shift(new Vector3(0, 0, dist))
                .Rotate(fullerene.EulerAngles)
                .RotateRev(EulerAngles);

            var faces = GenerateFacesIndices();

            return GenerateVerticesPositions()
                    .AddMidPoints(faces)
                    .Any(fullerene.Contains);
        }

        public virtual bool Contains(Vector3 point)
        {
            if (FigureCollision.SpheresInside(point, 0, Center, GenerateInnerSphereRadius()))
                return true;

            if (!FigureCollision.SpheresInside(point, 0, Center, GenerateOuterSphereRadius()))
                return false;

            var vertices = GenerateVerticesPositions();

            foreach (var face in GenerateFacesIndices())
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
