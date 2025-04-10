using System.Numerics;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.Fullerenes;

namespace Fullerenes.Server.Objects.LimitedAreas
{
    public abstract class LimitedArea(
        float x, float y, float z, (string name, float param)[] parameters,
        int numberOfFullerene, Func<int, Fullerene>? produceFullerene)
    {
        protected static readonly int RetryCountMax = 100;
        public Vector3 Center { get; init; } = new(x, y, z);
        public (string name, float param)[] Params { get; init; } = parameters;
        public int RequestedNumberOfFullerenes { get; set; } = numberOfFullerene;
        public Func<int, Fullerene>? ProduceFullerene { get; set; } = produceFullerene;
        public abstract IEnumerable<Fullerene> GenerateFullerenes(
            int seriesFs, Octree<Parallelepiped, Fullerene> octree, bool clear = false);
        public abstract bool Contains(Fullerene fullerene);
        public abstract float GenerateOuterRadius();
    }
}
