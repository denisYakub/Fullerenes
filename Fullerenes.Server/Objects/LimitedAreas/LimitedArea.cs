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
        int numberOfFullerene, Func<int, Fullerene>? produceFullerene)
    {
        protected static readonly int RetryCountMax = 100;
        [Key(0)]
        public Vector3 Center { get; init; } = new(x, y, z);
        [Key(1)]
        public (string name, float param)[] Params { get; init; } = parameters;
        [Key(2)]
        public IReadOnlyCollection<Fullerene>? Fullerenes { get; set; } 
        [IgnoreMember]
        public int RequestedNumberOfFullerenes { get; set; } = numberOfFullerene;
        [IgnoreMember]
        public Func<int, Fullerene>? ProduceFullerene { get; set; } = produceFullerene;
        public abstract IEnumerable<Fullerene> GenerateFullerenes(
            int seriesFs, Octree<Parallelepiped, Fullerene> octree, bool clear = false);
        public abstract bool Contains(Fullerene fullerene);
        public abstract float GenerateOuterRadius();
    }
}
