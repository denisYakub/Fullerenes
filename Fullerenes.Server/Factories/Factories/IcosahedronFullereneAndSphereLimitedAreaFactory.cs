using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;

namespace Fullerenes.Server.Factories.Factories
{
    public class IcosahedronFullereneAndSphereLimitedAreaFactory(CreateFullerenesAndLimitedAreaRequest request) : FullereneAndLimitedAreaFactory(request)
    {
        public override LimitedArea CreateLimitedArea()
        {
            return new SphereLimitedArea(Request.AreaX, Request.AreaY, Request.AreaZ, Request.AreaAdditionalParams[0], Request.NumberOfF, CreateFullerene);
        }

        public override Fullerene CreateFullerene(int limitedAreaId, int series)
        {
            return new IcosahedronFullerene(
                Request.AreaX - Request.AreaAdditionalParams[0], Request.AreaX + Request.AreaAdditionalParams[0],
                Request.AreaY - Request.AreaAdditionalParams[0], Request.AreaY + Request.AreaAdditionalParams[0],
                Request.AreaZ - Request.AreaAdditionalParams[0], Request.AreaZ + Request.AreaAdditionalParams[0],
                Request.MaxAlphaF, Request.MaxBetaF, Request.MaxGammaF,
                Request.MinSizeF, Request.MaxSizeF,
                Request.Shape, Request.Scale,
                limitedAreaId, series);
        }
    }
}
