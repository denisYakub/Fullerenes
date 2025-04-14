using Fullerenes.Server.Geometry;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Fullerenes;
using MessagePack;

namespace Fullerenes.Server.Objects.LimitedAreas
{
    [MessagePackObject]
    public class SphereLimitedArea(
        float x, float y, float z, float r,
        IOctree<Fullerene> octree, int series,
        Func<Fullerene> produceFullerene) : LimitedArea(
              x, y, z, [("Radius", r)], 
              octree, series,
              produceFullerene)
    {
        public SphereLimitedArea() : this(0, 0, 0, 0, null, 0, null) { }
        public override float GenerateOuterRadius() => Params[0].param;

        public override bool Contains(Fullerene fullerene)
        {
            ArgumentNullException.ThrowIfNull(fullerene);

            return
                FiguresCollision.SpheresInside(
                    fullerene.Center, fullerene.GenerateOuterSphereRadius(),
                    Center, Params[0].param);
        }

        public override string ToString()
        {
            return
                "Center: " + Center + ", " +
                "Parameters: " + string.Join(", ", Params.Select(val => val.name + ": " + val.param));
        }
    }
}