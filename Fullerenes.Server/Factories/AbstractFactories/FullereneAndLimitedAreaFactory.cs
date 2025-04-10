using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.Enums;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;

namespace Fullerenes.Server.Factories.AbstractFactories
{
    public abstract class FullereneAndLimitedAreaFactory(
        int numberOfSeries, int numberOfFullerenes, 
        float minSizeFullerene, float maxSizeFullerene,
        AreaTypes areaType, FullereneTypes fullereneType)
    {
        public int NumberOfSeries { get; init; } = numberOfSeries;
        public int NumberOfFullerenes { get; init; } = numberOfFullerenes;
        public AreaTypes AreaType { get; init; } = areaType;
        public FullereneTypes FullereneType { get; init; } = fullereneType;
        public (float MinSizeFullerene, float MaxSizeFullerenes) FullereneSizeRange { get; init; } = new(minSizeFullerene, maxSizeFullerene);
        public abstract LimitedArea CreateLimitedArea();
        public abstract Fullerene CreateFullerene(int series);
    }
}
