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

        private static readonly float Phi = (1 + MathF.Sqrt(5)) / 2;

        public override IReadOnlyCollection<int[]> Faces => _faces;
        private static readonly int[][] _faces = [
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

        public override ICollection<Vector3> Vertices
        {
            get
            {
                return GenerateDefaultVerticesPositions(Size)
                    .AddMidPoints(in _faces, 12)
                    .Rotate(EulerAngles)
                    .Shift(Center);
            }
        }

        public override float OuterSphereRadius 
            => 0.951f * GetEdgeSize();

        public override float InnerSphereRadius
            => 0.7557f * GetEdgeSize();

        public override float GetEdgeSize()
            => Vector3.Distance(
                new Vector3(-1, Phi, 0) * Size,
                new Vector3(-Phi, 0, 1) * Size);

        private static Vector3[] GenerateDefaultVerticesPositions(float size)
        {
            var array = new Vector3[42];

            array[0] = new Vector3(-1, Phi, 0) * size;
            array[1] = new Vector3(1, Phi, 0) * size;
            array[2] = new Vector3(-1, -Phi, 0) * size;
            array[3] = new Vector3(1, -Phi, 0) * size;

            array[4] = new Vector3(0, -1, Phi) * size;
            array[5] = new Vector3(0, 1, Phi) * size;
            array[6] = new Vector3(0, -1, -Phi) * size;
            array[7] = new Vector3(0, 1, -Phi) * size;

            array[8] = new Vector3(Phi, 0, -1) * size;
            array[9] = new Vector3(Phi, 0, 1) * size;
            array[10] = new Vector3(-Phi, 0, -1) * size;
            array[11] = new Vector3(-Phi, 0, 1) * size;

            return array;
        }

        public override float GenerateVolume()
        {
            int numberOfDotsInsideFullerene = 0;
            int samples = 1_000_000;
            float radius = OuterSphereRadius;

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

