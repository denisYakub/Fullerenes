using System.Numerics;
using Fullerenes.Server.Extensions;
using MathNet.Numerics.Distributions;

namespace Fullerenes.Server.Objects.Fullerenes
{
    public class IcosahedronFullerene(
        float x, float y, float z, 
        float alpha, float beta, float gamma, 
        float size) 
        : Fullerene(
            x, y, z, 
            alpha, beta, gamma, 
            size)
    {
        public IcosahedronFullerene() 
            : this(0, 0, 0, 0, 0, 0, 0) { }

        private readonly object _lock = new();
        private static readonly float Phi = (1 + MathF.Sqrt(5)) / 2;

        public override IReadOnlyCollection<int[]> Faces => _faces;
        private static readonly IReadOnlyCollection<int[]> _faces = [
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
            [9, 8, 1]
        ];

        private ICollection<Vector3>? _vertices;
        public override ICollection<Vector3> Vertices
        {
            get
            {
                lock (_lock)
                {
                    return _vertices ??=
                        GenerateDefaultVerticesPositions(Size)
                        .Rotate(EulerAngles)
                        .Shift(Center);
                }
            }
        }

        public override float GenerateOuterSphereRadius() 
            => 0.951f * GetEdgeSize();

        public override float GenerateInnerSphereRadius()
            => 0.7557f * GetEdgeSize();

        public override float GetEdgeSize()
            => Vector3.Distance(
                new Vector3(-1, Phi, 0) * Size,
                new Vector3(-Phi, 0, 1) * Size);

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

        public override float GenerateVolume()
        {
            int numberOfDotsInsideFullerene = 0;
            int samples = 1_000_000;
            float radius = GenerateOuterSphereRadius();

            var random = new Random();

            for (int i = 0; i < samples; i++)
            {
                Vector3 dot;

                do
                {
                    float x = (float)(random.NextDouble() * 2 - 1);
                    float y = (float)(random.NextDouble() * 2 - 1);
                    float z = (float)(random.NextDouble() * 2 - 1);

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

        public override string ToString()
        {
            return string.Join(", ", Vertices);
        }
    }
}

