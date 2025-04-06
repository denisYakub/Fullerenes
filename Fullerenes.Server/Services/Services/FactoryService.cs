using Azure.Core;
using Fullerenes.Server.Extensions;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Factories.Factories;
using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.Enums;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Services.IServices;
using MathNet.Numerics.Distributions;

namespace Fullerenes.Server.Services.Services
{
    public class FactoryService : IFactoryService
    {
        public FullereneAndLimitedAreaFactory GetFactory(AreaTypes areaType, FullereneTypes fullereneType, CreateFullerenesAndLimitedAreaRequest request)
        {
            float areaRadius = 0;

            if (request.AreaAdditionalParams.Nc is not null)
            {

                float nc = (float)request.AreaAdditionalParams.Nc;

                float sumVFullerenes = 
                    GenerateAvgSumOfFullerenes(
                        request.NumberOfF, fullereneType, 
                        request.MinSizeF, request.MaxSizeF, 
                        request.Shape, request.Scale);

                switch (areaType)
                {
                    case AreaTypes.Sphere:
                        areaRadius = MathF.Pow(sumVFullerenes / nc, 1f / 3f);
                        break;
                    default:
                        throw new NotImplementedException("We are not working with this type of limited area!");
                }
            }

            return (typeLA: areaType, typeF: fullereneType) switch
            {
                (AreaTypes.Sphere, FullereneTypes.Icosahedron) => new IcosahedronFullereneAndSphereLimitedAreaFactory(
                    request.AreaX, request.AreaY, request.AreaZ, areaRadius,
                    request.NumberOfSeries, request.NumberOfF,
                    request.MaxAlphaF, request.MaxBetaF, request.MaxGammaF,
                    request.MinSizeF, request.MaxSizeF,
                    request.Shape, request.Scale
                    ),
                _ => throw new NotImplementedException("We are not working with this type of limited area and fullerenes!")
            };
        }

        private static float GenerateAvgSumOfFullerenes(
            int numberOfFullerenes, FullereneTypes fullereneType,
            float minSizeFullerene, float maxSizeFullerene,
            float shape, float scale)
        {
            float fullereneSize = 
                new Gamma(shape, scale)
                .GetGammaRandoms(minSizeFullerene, maxSizeFullerene)
                .First();

            return numberOfFullerenes * 
                fullereneType switch
                {
                    FullereneTypes.Icosahedron
                    => new IcosahedronFullerene(0, 0, 0, 0, 0, 0, fullereneSize).GenerateVolume(),
                    _
                    => throw new NotImplementedException("We are not working with this type of fullerenes!")
                };

        }
    }
}
