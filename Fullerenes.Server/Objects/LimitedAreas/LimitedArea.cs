using System.Numerics;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.Fullerenes;
using MessagePack;

namespace Fullerenes.Server.Objects.LimitedAreas
{
    [MessagePackObject]
    [Union(0, typeof(SphereLimitedArea))]
    public abstract class LimitedArea(
        float x, float y, float z, (string name, float param)[] parameters,
        Octree<Parallelepiped, Fullerene> octree, int series,
        Func<Fullerene>? produceFullerene)
    {
        protected static readonly int RetryCountMax = 100;
        protected Octree<Parallelepiped, Fullerene> Octree = octree;
        [Key(0)]
        public Vector3 Center { get; set; } = new(x, y, z);
        [Key(1)]
        public (string name, float param)[] Params { get; set; } = parameters;
        [Key(2)]
        public IEnumerable<Fullerene>? Fullerenes { get; set; }
        [Key(3)]
        public int Series { get; set; } = series;
        [IgnoreMember]
        public Func<Fullerene>? ProduceFullerene { get; set; } = produceFullerene;
        [IgnoreMember]
        public static bool ClearOctreeCollection { get; set; } = false;
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
                    Octree.ClearCurrentThreadCollection(Series);
            }
        }
        protected virtual Fullerene? TryToGenerateFullerene()
        {
            var fullerene = ProduceFullerene?.Invoke() ?? null;

            return
                fullerene is not null &&
                Contains(fullerene) &&
                Octree.AddData(fullerene, Series, fullerene.Intersect, fullerene.Inside, fullerene.PartInside)
                ? fullerene
                : null;
        }
    }
}
