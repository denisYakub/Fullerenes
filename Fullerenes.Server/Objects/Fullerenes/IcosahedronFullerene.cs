using System.Numerics;
using Fullerenes.Server.Extensions;
using Fullerenes.Server.Geometry;
using Fullerenes.Server.Objects.CustomStructures;
using MathNet.Numerics.Distributions;

namespace Fullerenes.Server.Objects.Fullerenes
{
    public class IcosahedronFullerene(float x, float y, float z, float alpha, float beta, float gamma, float size)
        : Fullerene(x, y, z, alpha, beta, gamma, size)
    {
        public IcosahedronFullerene() : this(0, 0, 0, 0, 0, 0, 0) { }

        public IcosahedronFullerene(
            float minX, float maxX, float minY,
            float maxY, float minZ, float maxZ,
            float maxAlpha, float maxBeta, float maxGamma,
            float minSize, float maxSize,
            float shape, float scale,
            int limitedAreaId, int series)
            : this(
                  new Random().GetEvenlyRandoms(minX, maxX).First(),
                  new Random().GetEvenlyRandoms(minY, maxY).First(),
                  new Random().GetEvenlyRandoms(minZ, maxZ).First(),
                  new Random().GetEvenlyRandoms(-maxAlpha, maxAlpha).First(),
                  new Random().GetEvenlyRandoms(-maxBeta, maxBeta).First(),
                  new Random().GetEvenlyRandoms(-maxGamma, maxGamma).First(),
                  new Gamma(shape, scale).GetGammaRandoms(minSize, maxSize).First()
            )
            => (LimitedAreaId, Series) = (limitedAreaId, series);

        private static readonly float Phi = (1 + MathF.Sqrt(5)) / 2;

        public override float GenerateInnerSphereRadius()
            => MathF.Pow(Phi, 2) * GenerateOuterSphereRadius() / MathF.Sqrt(12);

        public override ICollection<int[]> GenerateFacesIndices()
            => [
                [0, 11, 5],
                [0, 5, 1],
                [0, 1, 7],
                [0, 7, 10],
                [0, 10, 11],
                [1, 5, 9],
                [5, 11, 4],
                [11, 10, 2],
                [10, 7, 6],
                [7, 1, 8],
                [3, 9, 4],
                [3, 4, 2],
                [3, 2, 6],
                [3, 6, 8],
                [3, 8, 9],
                [4, 9, 5],
                [2, 4, 11],
                [6, 2, 10],
                [8, 6, 7],
                [9, 8, 1],
            ];

        public override ICollection<Vector3> GenerateStartVerticesPositions() =>
            [
                new Vector3(-1,  Phi, 0) * Size,
                new Vector3( 1,  Phi, 0) * Size,
                new Vector3(-1, -Phi, 0) * Size,
                new Vector3( 1, -Phi, 0) * Size,

                new Vector3(0, -1,  Phi) * Size,
                new Vector3(0,  1,  Phi) * Size,
                new Vector3(0, -1, -Phi) * Size,
                new Vector3(0,  1, -Phi) * Size,

                new Vector3( Phi, 0, -1) * Size,
                new Vector3( Phi, 0,  1) * Size,
                new Vector3(-Phi, 0, -1) * Size,
                new Vector3(-Phi, 0,  1) * Size
            ];

        public override bool Inside(Parallelepiped parallelepiped)
        {
            ArgumentNullException.ThrowIfNull(parallelepiped);

            var outerSphereRadius = GenerateOuterSphereRadius();

            var result = parallelepiped.Center.X - parallelepiped.Length / 2 <= Center.X - outerSphereRadius &&
                Center.X + outerSphereRadius <= parallelepiped.Center.X + parallelepiped.Length / 2 &&
                parallelepiped.Center.Y - parallelepiped.Height / 2 <= Center.Y - outerSphereRadius &&
                Center.Y + outerSphereRadius <= parallelepiped.Center.Y + parallelepiped.Height / 2 &&
                parallelepiped.Center.Z - parallelepiped.Width / 2 <= Center.Z - outerSphereRadius &&
                Center.Z + outerSphereRadius <= parallelepiped.Center.Z + parallelepiped.Width / 2;

            return result;
        }
        public override bool PartInside(Parallelepiped parallelepiped)
        {
            ArgumentNullException.ThrowIfNull(parallelepiped);

            return FigureCollision.Intersects(parallelepiped, Center, GenerateOuterSphereRadius());
        }
    }
}

