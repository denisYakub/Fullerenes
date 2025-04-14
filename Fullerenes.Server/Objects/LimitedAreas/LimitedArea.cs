using System.Numerics;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Fullerenes;
using MessagePack;

namespace Fullerenes.Server.Objects.LimitedAreas
{
    [MessagePackObject]
    [Union(0, typeof(SphereLimitedArea))]
    public abstract class LimitedArea(
        float x, float y, float z, 
        (string name, float value)[] parameters,
        IOctree<Fullerene> octree, int series,
        Func<Fullerene>? produceFullerene)
    {
        [Key(0)]
        public Vector3 Center { get; } = new(x, y, z);
        [Key(1)]
        public (string name, float value)[] Params { get; } = parameters;
        [Key(2)]
        public int Series { get; } = series;
        [Key(3)]
        public IEnumerable<Fullerene>? Fullerenes { get; set; }

        protected Func<Fullerene>? ProduceFullerene { get; } = produceFullerene;
        protected IOctree<Fullerene> Octree = octree;

        protected static bool ClearOctreeCollection { get; set; }
        protected static readonly int RetryCountMax = 100;

        public abstract bool Contains(Fullerene fullerene);
        public abstract float GenerateOuterRadius();
        public abstract string SaveToCsv(string folderPath);

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
