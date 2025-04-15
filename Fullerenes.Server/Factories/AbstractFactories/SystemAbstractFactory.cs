using Fullerenes.Server.Objects.Adapters;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.Enums;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;

namespace Fullerenes.Server.Factories.AbstractFactories
{
    public abstract class SystemAbstractFactory(ILimitedAreaAdapter adapter)
    {
        public abstract AreaTypes AreaType { get; init; }
        public abstract FullereneTypes FullereneType { get; init; }
        public abstract required int ThreadNumber {  get; set; }
        public abstract required int FullerenesNumber { get; set; }
        public abstract IOctree<Fullerene> GenerateOctree();
        public abstract Fullerene GenerateFullerene();
        public abstract LimitedArea GenerateLimitedArea(int thread, IOctree<Fullerene> octree);
        public string SaveLimitedArea(LimitedArea limitedArea, long generationId)
        {
            ArgumentNullException.ThrowIfNull(limitedArea);

            return adapter.Write([ limitedArea ], $"{generationId}_{limitedArea.Series}");
        }
    }
}
