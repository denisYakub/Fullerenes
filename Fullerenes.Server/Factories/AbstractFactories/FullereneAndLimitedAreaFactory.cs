using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;

namespace Fullerenes.Server.Factories.AbstractFactories
{
    public abstract class FullereneAndLimitedAreaFactory(CreateFullerenesAndLimitedAreaRequest request)
    {
        public CreateFullerenesAndLimitedAreaRequest Request { get; init; } = request;
        public abstract LimitedArea CreateLimitedArea();
        public abstract Fullerene CreateFullerene(int limitedAreaId, int series);
    }
}
