using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;

namespace Fullerenes.Server.Factories.Factories
{
    public class IcosahedronFullereneAndSphereLimitedAreaFactory(
        float areaX, float areaY, float areaZ, float areaR,
        int numberOfSeries, int numberOfFullerenes,
        float maxPracessioAngle, float maxNutationAngle, float maxProperRotationAngle,
        float minFullereneSize, float maxFullereneSize,
        float shape, float scale) 
        : FullereneAndLimitedAreaFactory(
            numberOfSeries, numberOfFullerenes,
            minFullereneSize, maxFullereneSize)
    {
        public override LimitedArea CreateLimitedArea()
        {
            return new SphereLimitedArea(areaX, areaY, areaZ, areaR, numberOfFullerenes, CreateFullerene);
        }

        public override Fullerene CreateFullerene(int limitedAreaId, int series)
        {
            return new IcosahedronFullerene(
                areaX - areaR, areaX + areaR,
                areaY - areaR, areaY + areaR,
                areaZ - areaR, areaZ + areaR,
                maxPracessioAngle, maxNutationAngle, maxProperRotationAngle,
                minFullereneSize, maxFullereneSize,
                shape, scale,
                limitedAreaId, series);
        }
    }
}
