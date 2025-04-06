using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;

namespace Fullerenes.Server.Factories.AbstractFactories
{
    public abstract class FullereneAndLimitedAreaFactory(
        int numberOfSeries, int numberOfFullerenes, 
        float minSizeFullerene, float maxSizeFullerene)
    {
        public int NumberOfSeries { get; init; } = numberOfSeries;
        public int NumberOfFullerenes { get; init; } = numberOfFullerenes;
        public (float MinSizeFullerene, float MaxSizeFullerenes) FullereneSizeRange { get; init; } = new(minSizeFullerene, maxSizeFullerene);
        public abstract LimitedArea CreateLimitedArea();
        public abstract Fullerene CreateFullerene(int limitedAreaId, int series);
    }
}
