using System;
using System.Diagnostics;
using System.Numerics;
using Fullerenes.Server.Extensions;
using Fullerenes.Server.Geometry;
using Fullerenes.Server.Objects.CustomStructures;
using MathNet.Numerics.Distributions;

namespace Fullerenes.Server.Objects.Fullerenes
{
    public class IcosahedronFullerene : Fullerene
    {
        public IcosahedronFullerene() 
            : base(0, 0, 0, 0, 0, 0, 0) { }

        public IcosahedronFullerene(float x, float y, float z, float alpha, float beta, float gamma, float size)
            : base(x, y, z, alpha, beta, gamma, size) { }

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

        public override ReadOnlySpan<Vector3> Vertices
        {
            get
            {
                Span<Vector3> span = new Vector3[42];

                int count = IcosahedronFullerene.FillVertices(Size, Center, EulerAngles, _faces, span);
                
                return span[..count];
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


        public static int FillVertices(
            float size, Vector3 center, EulerAngles angles, 
            int[][] faces, Span<Vector3> span)
        {
            float phi = (1 + MathF.Sqrt(5)) / 2;

            span[0] = new Vector3(-1, phi, 0) * size;
            span[1] = new Vector3(1, phi, 0) * size;
            span[2] = new Vector3(-1, -phi, 0) * size;
            span[3] = new Vector3(1, -phi, 0) * size;

            span[4] = new Vector3(0, -1, phi) * size;
            span[5] = new Vector3(0, 1, phi) * size;
            span[6] = new Vector3(0, -1, -phi) * size;
            span[7] = new Vector3(0, 1, -phi) * size;

            span[8] = new Vector3(phi, 0, -1) * size;
            span[9] = new Vector3(phi, 0, 1) * size;
            span[10] = new Vector3(-phi, 0, -1) * size;
            span[11] = new Vector3(-phi, 0, 1) * size;

            int index = 12;

            var edgeSet = new HashSet<int>();

            static int EncodeEdge(int a, int b) => (Math.Min(a, b) << 10) | Math.Max(a, b);

            foreach (var face in faces)
            {
                for (int i = 0; i < face.Length; i++)
                {
                    if (index >= span.Length)
                        break;

                    int v1 = face[i];
                    int v2 = face[(i + 1) % face.Length];

                    int code = EncodeEdge(v1, v2);
                    if (!edgeSet.Add(code)) continue;

                    span[index++] = (span[v1] + span[v2]) * 0.5f;
                }
            }

            var matrix = Formulas.CreateRotationMatrix(
                angles.PraecessioAngle,
                angles.NutatioAngle,
                angles.ProperRotationAngle);

            for (int i = 0; i < index; i++)
            {
                span[i] = Vector3.Transform(span[i], matrix);
                span[i] += center;
            }

            return index;
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
    }
}

