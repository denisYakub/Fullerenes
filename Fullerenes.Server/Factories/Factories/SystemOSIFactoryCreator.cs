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
        public override SystemAbstractFactory CreateSystemFactory(CreateFullerenesAndLimitedAreaRequest request, int fullereneNumber)
        {
            var areaRadius = GetAreaRadius(
                request.AreaAdditionalParams,
                fullereneNumber,
                request.MinSizeF, request.MaxSizeF,
                request.Shape, request.Scale);

            var areaCenter = new Vector3(request.AreaX, request.AreaY, request.AreaZ);

            var startArea = new CubeRegion()
            {
                Center = areaCenter,
                Edge = 2 * areaRadius,
            };

            var fullereneMinCenter = new Vector3()
            {
                X = areaCenter.X - areaRadius,
                Y = areaCenter.Y - areaRadius,
                Z = areaCenter.Z - areaRadius
            };

            var fullereneMaxCenter = new Vector3()
            {
                X = areaCenter.X + areaRadius,
                Y = areaCenter.Y + areaRadius,
                Z = areaCenter.Z + areaRadius
            };

            var rotationAngles = new EulerAngles()
            {
                PraecessioAngle = request.MaxAlphaF,
                NutatioAngle = request.MaxBetaF,
                ProperRotationAngle = request.MaxGammaF
            };

            return new SystemOSIFactory
            { 
                StartRegion = startArea,
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
                var nc = (float)request.Nc;

                var sumVFullerenes = GenerateSumVAvgFullerenes(
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
            var avgSize = new Gamma(shape, scale)
                .GetGammaRandoms(minSize, maxSize)
                .Take(number)
                .Average();

            return number * new IcosahedronFullerene(0, 0, 0, 0, 0, 0, avgSize).GenerateVolume();
        }
    }
}
