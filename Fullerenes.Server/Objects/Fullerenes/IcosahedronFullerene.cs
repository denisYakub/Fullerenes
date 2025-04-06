using System;
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
        public IcosahedronFullerene() 
            : this(0, 0, 0, 0, 0, 0, 0) { }

        public IcosahedronFullerene(
            float minX, float maxX, float minY,
            float maxY, float minZ, float maxZ,
            float maxAlpha, float maxBeta, float maxGamma,
            float minSize, float maxSize,
            float shape, float scale,
            int limitedAreaId, int series)
            : this(
                  Random.GetEvenlyRandoms(minX, maxX).First(),
                  Random.GetEvenlyRandoms(minY, maxY).First(),
                  Random.GetEvenlyRandoms(minZ, maxZ).First(),
                  Random.GetEvenlyRandoms(-maxAlpha, maxAlpha).First(),
                  Random.GetEvenlyRandoms(-maxBeta, maxBeta).First(),
                  Random.GetEvenlyRandoms(-maxGamma, maxGamma).First(),
                  new Gamma(shape, scale).GetGammaRandoms(minSize, maxSize).First())
            => (LimitedAreaId, Series) = (limitedAreaId, series);

        private static readonly float Phi = (1 + MathF.Sqrt(5)) / 2;

        private readonly Lazy<IReadOnlyCollection<int[]>> _faces = new(() => FiguresFaces.IcosahedronFacesIndices);
        private ICollection<Vector3>? _vertices;

        public override ICollection<Vector3> Vertices
        {
            get
            {
                lock (Lock)
                {
                    return _vertices ??=
                        GenerateDefaultVerticesPositions(Size)
                        .Rotate(EulerAngles)
                        .Shift(Center);
                }
            }
        }

        public override IReadOnlyCollection<int[]> Faces => _faces.Value;

        public override float GenerateOuterSphereRadius() => Size;

        public override float GenerateInnerSphereRadius()
            => MathF.Pow(Phi, 2) * Size / MathF.Sqrt(12);

        private static ICollection<Vector3> GenerateDefaultVerticesPositions(float size)
        {
            return
            [
                new Vector3(-1, Phi, 0) * size,
                new Vector3( 1, Phi, 0) * size,
                new Vector3(-1, -Phi, 0) * size,
                new Vector3( 1, -Phi, 0) * size,

                new Vector3(0, -1, Phi) * size,
                new Vector3(0,  1, Phi) * size,
                new Vector3(0, -1, -Phi) * size,
                new Vector3(0,  1, -Phi) * size,

                new Vector3(Phi, 0, -1) * size,
                new Vector3(Phi, 0,  1) * size,
                new Vector3(-Phi, 0, -1) * size,
                new Vector3(-Phi, 0,  1) * size
            ];
        }

        public override bool Inside(Parallelepiped parallelepiped)
        {
            ArgumentNullException.ThrowIfNull(parallelepiped);

            var outerSphereRadius = Size;

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

            if (!FiguresCollision.Intersects(parallelepiped, Center, GenerateOuterSphereRadius()))
                return false;

            if (FiguresCollision.Intersects(parallelepiped, Center, GenerateInnerSphereRadius()))
                return true;

            return
                Vertices
                .AddMidPoints(Faces)
                .Any(vertex => FiguresCollision.Pointinside(parallelepiped, vertex));
        }

        public override float GenerateVolume()
        {
            int numberOfDotsInsideFullerene = 0;
            const int numberOfDots = 1_000_000;

            var dots = Random.GetEvenlyRandoms(-Size, Size).Take(numberOfDots * 3).ToArray();


            for (int i = 0; i < numberOfDots * 3; i += 3)
            {
                if (Contains(new Vector3(dots[i], dots[i + 1], dots[i + 2])))
                    numberOfDotsInsideFullerene++;
            }

            return MathF.Pow(2 * Size, 3) * numberOfDotsInsideFullerene / numberOfDots;
        }
    }
}

