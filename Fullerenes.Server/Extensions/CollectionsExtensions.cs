using System.Numerics;
using Fullerenes.Server.Geometry;
using Fullerenes.Server.Objects.CustomStructures;

namespace Fullerenes.Server.Extensions
{
    public static class CollectionsExtensions
    {
        public static List<Vector3> AddMidPoints(this List<Vector3> vertices, in int[][] facesIndices)
        {
            var edgeSet = new HashSet<(int, int)>();

            foreach (var face in facesIndices)
            {
                for (int i = 0; i < face.Length; i++)
                {
                    int v1 = face[i];
                    int v2 = face[(i + 1) % face.Length];
                    var edge = (Math.Min(v1, v2), Math.Max(v1, v2));

                    if (!edgeSet.Add(edge)) continue;

                    Vector3 midpoint = (vertices.ElementAt(v1) + vertices.ElementAt(v2)) / 2;
                    vertices.Add(midpoint);
                }
            }

            return vertices;
        }
        public static List<Vector3> Shift(this List<Vector3> vertices, Vector3 dot)
        {
            for (int i = 0; i < vertices.Count; i++) 
            { 
                vertices[i] += dot;
            }

            return vertices;
        }

        public static List<Vector3> Rotate(this List<Vector3> vertices, EulerAngles angles)
        {
            var rotationMatrix = Formulas.CreateRotationMatrix(
                angles.PraecessioAngle,
                angles.NutatioAngle,
                angles.ProperRotationAngle);

            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = Vector3.Transform(vertices[i], rotationMatrix);

            return vertices;
        }
    }
}
