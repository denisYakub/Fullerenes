using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.Fullerenes;

namespace Fullerenes.Server.Objects.LimitedAreas
{
    public abstract class LimitedArea(float x, float y, float z, int numberOfFullerene, Func<int, int, Fullerene>? produceFullerene)
    {
        public int Id { get; set; }
        public Vector3 Center { get; init; } = new(x, y, z);
        public int RealNumberOfFullerenes { get { return Fullerenes?.Count ?? 0; } }
        public int RequestedNumberOfFullerenes { get; set; } = numberOfFullerene;
        public ICollection<Fullerene>? Fullerenes { get; init; }
        [NotMapped]
        protected static readonly int RetryCountMax = 100;
        [NotMapped]
        public Func<int, int, Fullerene>? ProduceFullerene { get; set; } = produceFullerene;
        public abstract float GenerateOuterRadius();
        public abstract bool Contains(Fullerene fullerene);
        public abstract IEnumerable<Fullerene> GenerateFullerenes(int seriesFs, Octree<Parallelepiped, Fullerene> octree);
    }
}
