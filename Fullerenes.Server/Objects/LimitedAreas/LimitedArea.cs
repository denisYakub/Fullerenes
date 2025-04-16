using System.Numerics;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Fullerenes;

namespace Fullerenes.Server.Objects.LimitedAreas
{
    public abstract class LimitedArea(
        float x, float y, float z, 
        (string name, float value)[] parameters,
        int series)
    {
        public Vector3 Center { get; } = new(x, y, z);
        public (string name, float value)[] Params { get; } = parameters;
        public int Series { get; } = series;
        public IEnumerable<Fullerene>? Fullerenes { get; set; }
        public required Func<Fullerene>  ProduceFullerene { get; init; }
        public required IOctree<Fullerene> Octree { get; init; }

        protected static bool ClearOctreeCollection { get; set; }
        protected static readonly int RetryCountMax = 100;

        public abstract bool Contains(Fullerene fullerene);
        public abstract float GenerateOuterRadius();

        public void StartGeneration(int fullerenesNumber)
        {
            Fullerenes = GenerateFullerenes().Take(fullerenesNumber);
        }

        protected virtual IEnumerable<Fullerene> GenerateFullerenes()
        {
            ArgumentNullException.ThrowIfNull(Octree);
            try
            {
                int reTryCount = 0;

                while (true)
                {
                    if (reTryCount == RetryCountMax)
                        yield break;

                    var fullerene = TryToGenerateFullerene();

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
                if (ClearOctreeCollection)
                    Octree.ClearThreadCollection(Series);
            }
        }
        protected virtual Fullerene? TryToGenerateFullerene()
        {
            var fullerene = ProduceFullerene?.Invoke() ?? null;

            return
                fullerene is not null &&
                Contains(fullerene) &&
                Octree.AddData(fullerene, Series, fullerene.Intersect)
                ? fullerene
                : null;
        }
    }
}
