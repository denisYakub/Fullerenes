using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.Enums;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;

namespace Fullerenes.Server.Factories.AbstractFactories
{
    public abstract class FullereneAndLimitedAreaFactory(
        int numberOfSeries, int numberOfFullerenes, 
        AreaTypes areaType, FullereneTypes fullereneType)
    {
        public int NumberOfSeries { get; init; } = numberOfSeries;
        public int NumberOfFullerenes { get; init; } = numberOfFullerenes;
        public FullereneTypes FullereneType { get; init; } = fullereneType;
        public AreaTypes AreaType { get; init; } = areaType;
        public IOctree<Fullerene> Octree { get; set; }
        public abstract LimitedArea CreateLimitedArea(int series);
        public abstract Fullerene CreateFullerene();
    }
}
