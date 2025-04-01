using System.Numerics;
using Fullerenes.Server.Geometry;
using Fullerenes.Server.Objects.CustomStructures;

namespace Fullerenes.Server.Extensions
{
    public static class CollectionsExtensions
    {
        public static ICollection<Vector3> AddMidPoints(this ICollection<Vector3> vertices, ICollection<int[]> facesIndices)
        {
            ArgumentNullException.ThrowIfNull(vertices);
            ArgumentNullException.ThrowIfNull(facesIndices);

            var edgeSet = new HashSet<(int, int)>();
            var newVertices = new List<Vector3>();

            foreach (var face in facesIndices)
            {
                for (int i = 0; i < face.Length; i++)
                {
                    int v1 = face[i];
                    int v2 = face[(i + 1) % face.Length];
                    var edge = (Math.Min(v1, v2), Math.Max(v1, v2));

                    if (!edgeSet.Add(edge)) continue;

                    Vector3 midpoint = (vertices.ElementAt(v1) + vertices.ElementAt(v2)) / 2;
                    newVertices.Add(midpoint);
                }
            }

            var verticesWithMidPoints = new List<Vector3>(vertices.Count + newVertices.Count);

            verticesWithMidPoints.AddRange(newVertices);

            return verticesWithMidPoints;
        }
        public static ICollection<Vector3> Shift(this ICollection<Vector3> vertices, Vector3 dot)
        {
            ArgumentNullException.ThrowIfNull(vertices);

            return vertices.Select(vertex => vertex + dot).ToArray();
        }

        public static ICollection<Vector3> Rotate(this ICollection<Vector3> vertices, EulerAngles angles)
        {
            var rotationMatrix = Formulas.CreateRotationMatrix(
                angles.PraecessioAngle,
                angles.NutatioAngle,
                angles.ProperRotationAngle);

            return vertices.Select(vertex => Vector3.Transform(vertex, rotationMatrix)).ToArray();
        }
        public static ICollection<Vector3> RotateRev(this ICollection<Vector3> vertices, EulerAngles angles)
        {
            var rotationMatrix = Formulas.CreateRotationMatrixReverse(
                angles.PraecessioAngle,
                angles.NutatioAngle,
                angles.ProperRotationAngle);

            return vertices.Select(vertex => Vector3.Transform(vertex, rotationMatrix)).ToArray();
        }
    }
}
