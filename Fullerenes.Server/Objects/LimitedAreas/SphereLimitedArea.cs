using Fullerenes.Server.Geometry;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.Fullerenes;

namespace Fullerenes.Server.Objects.LimitedAreas
{
    public class SphereLimitedArea : LimitedArea
    {
        public SphereLimitedArea(
            float x, float y, float z, float r, 
            int numberOfFullerene, Func<int, Fullerene> produceFullerene)
            : base(
                  x, y, z, [("Radius", r)], 
                  numberOfFullerene, produceFullerene) 
        { }

        public override float GenerateOuterRadius() => Params[0].param;

        public override bool Contains(Fullerene fullerene)
        {
            ArgumentNullException.ThrowIfNull(fullerene);

            return
                FiguresCollision.SpheresInside(
                    fullerene.Center, fullerene.GenerateOuterSphereRadius(),
                    Center, Params[0].param);
        }

        public override IEnumerable<Fullerene> GenerateFullerenes(
            int seriesFs, Octree<Parallelepiped, Fullerene> octree, bool clear = false)
        {
            ArgumentNullException.ThrowIfNull(octree);
            try
            {
                int reTryCount = 0;

                while (true)
                {
                    if (reTryCount == RetryCountMax)
                        yield break;

                    var fullerene = TryToGenerateFullerene(seriesFs, octree);

                    if (fullerene != null)
                    {
                        reTryCount = 0;

                        yield return fullerene;

                    }
                    else
                    {
                        reTryCount++;
                    }
                }
            }
            finally  
            {
                if(clear)
                    octree.ClearCurrentThreadCollection(seriesFs);
            }
        }

        public override string ToString()
        {
            return
                "Center: " + Center + ", " +
                "Parameters: " + string.Join(", ", Params.Select(val => val.name + ": " + val.param));
        }

        private Fullerene? TryToGenerateFullerene(int series, Octree<Parallelepiped, Fullerene> octree)
        {
            var fullerene = ProduceFullerene?.Invoke(series) ?? null;

            return 
                fullerene is not null && 
                Contains(fullerene) && 
                octree.AddData(fullerene, series, fullerene.Intersect, fullerene.Inside, fullerene.PartInside)
                ? fullerene
                : null;
        }
    }
}