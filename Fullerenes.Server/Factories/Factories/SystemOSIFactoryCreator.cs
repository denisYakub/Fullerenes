using Fullerenes.Server.Extensions;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.Fullerenes;
using MathNet.Numerics.Distributions;
using System.Numerics;

namespace Fullerenes.Server.Factories.Factories
{
    public class SystemOSIFactoryCreator : SystemAbstractFactoryCreator
    {
        public override SystemAbstractFactory CreateSystemFactory(CreateFullerenesAndLimitedAreaRequest request, int series, int fullereneNumber)
        {
            float areaRadius = GetAreaRadius(
                request.AreaAdditionalParams,
                fullereneNumber,
                request.MinSizeF, request.MaxSizeF,
                request.Shape, request.Scale);

            Vector3 areaCenter = new(request.AreaX, request.AreaY, request.AreaZ);

            Cube startArea = new()
            {
                Center = areaCenter,
                Height = 2 * areaRadius,
                Width = 2 * areaRadius,
                Length = 2 * areaRadius
            };

            Vector3 fullereneMinCenter = new()
            {
                X = areaCenter.X - areaRadius,
                Y = areaCenter.Y - areaRadius,
                Z = areaCenter.Z - areaRadius
            };

            Vector3 fullereneMaxCenter = new()
            {
                X = areaCenter.X + areaRadius,
                Y = areaCenter.Y + areaRadius,
                Z = areaCenter.Z + areaRadius
            };

            EulerAngles rotationAngles = new()
            {
                PraecessioAngle = request.MaxAlphaF,
                NutatioAngle = request.MaxBetaF,
                ProperRotationAngle = request.MaxGammaF
            };

            return new SystemOSIFactory<Cube>
            { 
                StartRegion = startArea,
                ThreadNumber = series,
                AreaCenter = areaCenter,
                AreaRadius = areaRadius,
                FullereneMinCenter = fullereneMinCenter,
                FullereneMaxCenter = fullereneMaxCenter,
                RotationAngles = rotationAngles,
                FullereneSize = (request.MinSizeF, request.MaxSizeF),
                FullereneSizeDistribution = (request.Shape, request.Scale),
                FullerenesNumber = fullereneNumber
            };
        }

        public static float GetAreaRadius(
            AreaAdditionalParamsRequest request,
            int numberOfFullerenes,
            float minSizeFullerne, float maxSizeFullerene,
            float shape, float scale)
        {
            if(request.Nc is not null)
            {
                float nc = (float)request.Nc;

                float sumVFullerenes = GenerateSumVAvgFullerenes(
                    numberOfFullerenes, 
                    minSizeFullerne, maxSizeFullerene, 
                    shape, scale);

                return MathF.Pow(sumVFullerenes / nc, 1f / 3f);

            } else if (request.AreaParams is not null)
            {
                return request.AreaParams[0];

            } else
            {
                throw new NotSupportedException("One of parameters in AreaAdditionalParamsRequest shouldnt be null!");
            }
        }

        private static float GenerateSumVAvgFullerenes(
            int number, float minSize, float maxSize, float shape, float scale)
        {
            float avgSize = new Gamma(shape, scale)
                .GetGammaRandoms(minSize, maxSize)
                .Take(number)
                .Average();

            return number * new IcosahedronFullerene(0, 0, 0, 0, 0, 0, avgSize).GenerateVolume();
        }
    }
}
