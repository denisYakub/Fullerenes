using System.Numerics;
using Fullerenes.Server.Geometry;
using Fullerenes.Server.Objects.CustomStructures;

namespace Fullerenes.Server.Extensions
{
    public static class CollectionsExtensions
    {
        public static ICollection<Vector3> AddMidPoints(this ICollection<Vector3> vertices, IReadOnlyCollection<int[]> facesIndices)
        {
            ArgumentNullException.ThrowIfNull(vertices);
            ArgumentNullException.ThrowIfNull(facesIndices);

            var edgeSet = new HashSet<(int, int)>();
            var result = new List<Vector3>(vertices);

            foreach (var face in facesIndices)
            {
                for (int i = 0; i < face.Length - 1; i++)
                {
                    int v1 = face[i];
                    int v2 = face[i + 1];
                    var edge = (Math.Min(v1, v2), Math.Max(v1, v2));

                    if (!edgeSet.Add(edge)) continue;

                    Vector3 midpoint = (vertices.ElementAt(v1) + vertices.ElementAt(v2)) / 2;
                    result.Add(midpoint);
                }
            }

            return result;
        }
        public static ICollection<Vector3> Shift(this ICollection<Vector3> vertices, Vector3 dot)
        {
            ArgumentNullException.ThrowIfNull(vertices);

            var result = new List<Vector3>(vertices.Count);

            foreach (var vertex in vertices)
            {
                result.Add(vertex + dot);
            }

            return result;
        }

        public static ICollection<Vector3> Rotate(this ICollection<Vector3> vertices, EulerAngles angles)
        {
            ArgumentNullException.ThrowIfNull(vertices);

            var rotationMatrix = Formulas.CreateRotationMatrix(
                angles.PraecessioAngle,
                angles.NutatioAngle,
                angles.ProperRotationAngle);

            var result = new List<Vector3>(vertices.Count);

            foreach (var vertex in vertices)
            {
                result.Add(Vector3.Transform(vertex, rotationMatrix));
            }

            return result;
        }
    }
}
