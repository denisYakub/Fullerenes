using Fullerenes.Server.Geometry;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Fullerenes;
using MessagePack;

namespace Fullerenes.Server.Objects.LimitedAreas
{
    public class SphereLimitedArea(float x, float y, float z, float r, int series) 
        : LimitedArea(x, y, z, [("Radius", r)], series)
    {
        public SphereLimitedArea() : this(0, 0, 0, 0, 0) { }

        public override bool Contains(Fullerene fullerene)
        {
            ArgumentNullException.ThrowIfNull(fullerene);

            return
                FiguresCollision.SpheresInside(
                    fullerene.Center, fullerene.GenerateOuterSphereRadius(),
                    Center, Params[0].value);
        }

        public override float GenerateOuterRadius() => Params[0].value;

        public override string SaveToCsv(string folderPath)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return
                "Center: " + Center + ", " +
                "Parameters: " + string.Join(", ", Params.Select(val => val.name + ": " + val.value));
        }
    }
}