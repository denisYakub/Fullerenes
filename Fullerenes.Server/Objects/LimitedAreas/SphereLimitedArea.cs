using Fullerenes.Server.Geometry;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.Fullerenes;

namespace Fullerenes.Server.Objects.LimitedAreas
{
    public class SphereLimitedArea : LimitedArea
    {
        public float Radius { get; set; }
        public SphereLimitedArea() : base(0, 0, 0, 0, null) => Radius = 0;
        public SphereLimitedArea(float x, float y, float z, float r, int numberOfFullerene, Func<int, int, Fullerene> produceFullerene)
            : base(x, y, z, numberOfFullerene, produceFullerene) => Radius = r;
        public SphereLimitedArea(float x, float y, float z, float r, IEnumerable<Fullerene> fullerenes)
            : base(x, y, z, 0, null) => (Radius, Fullerenes) = (r, fullerenes.ToList());
        public override float GenerateOuterRadius() => Radius;
        public override IEnumerable<Fullerene> GenerateFullerenes(int seriesFs, Octree<Parallelepiped, Fullerene> octree)
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
                //octree.ClearCurrentThreadCollection(seriesFs);
            }
        }

        public override bool Contains(Fullerene fullerene)
        {
            ArgumentNullException.ThrowIfNull(fullerene);

            return FiguresCollision.SpheresInside(fullerene.Center, fullerene.GenerateOuterSphereRadius(), Center, Radius);
        }

        private Fullerene? TryToGenerateFullerene(int series, Octree<Parallelepiped, Fullerene> octree)
        {
            var fullerene = ProduceFullerene?.Invoke(Id, series) ?? null;

            return 
                fullerene is not null && 
                Contains(fullerene) && 
                octree.AddData(fullerene, series, fullerene.Intersect, fullerene.Inside, fullerene.PartInside)
                ? fullerene
                : null;
        }
    }
}