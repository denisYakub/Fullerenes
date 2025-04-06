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

        public override float GenerateOuterSphereRadius() 
            => 0.951f * GetEdgeSize();

        public override float GenerateInnerSphereRadius()
            => 0.7557f * GetEdgeSize();

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
            int samples = 1_000_000;
            float radius = GenerateOuterSphereRadius();

            for (int i = 0; i < samples; i++)
            {
                Vector3 dot;

                do
                {
                    float x = (float)(Random.NextDouble() * 2 - 1);
                    float y = (float)(Random.NextDouble() * 2 - 1);
                    float z = (float)(Random.NextDouble() * 2 - 1);

                    dot = new Vector3(x, y, z);
                }
                while (dot.LengthSquared() > 1);

                dot *= radius;

                if(Contains(dot))
                    numberOfDotsInsideFullerene++;
            }

            float outerSphereVolume = (4f / 3f) * MathF.PI * MathF.Pow(radius, 3);

            return outerSphereVolume * numberOfDotsInsideFullerene / samples;
        }

        public override float GetEdgeSize()
        {
            var a = Vertices.ElementAt(Faces.ElementAt(0)[0]);
            var b = Vertices.ElementAt(Faces.ElementAt(0)[1]);

            return Vector3.Distance(
                Vertices.ElementAt(Faces.ElementAt(0)[0]), 
                Vertices.ElementAt(Faces.ElementAt(0)[1]));
        }
    }
}

