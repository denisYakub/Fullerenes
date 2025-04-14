using CsvHelper;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.Enums;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using System.Numerics;

namespace Fullerenes.Server.Factories.Factories
{
    public class IcosahedronFullereneAndSphereLimitedAreaFactory : FullereneAndLimitedAreaFactory
    {
        public IcosahedronFullereneAndSphereLimitedAreaFactory(
            float areaX, float areaY, float areaZ, float areaR,
            int numberOfSeries, int numberOfFullerenes,
            float maxPracessioAngle, float maxNutationAngle, float maxProperRotationAngle,
            float minFullereneSize, float maxFullereneSize,
            float shape, float scale
            )
            : base(
                  numberOfSeries, numberOfFullerenes,
                  AreaTypes.Sphere, FullereneTypes.Icosahedron)
        {
            Octree = new Octree<Fullerene>(
                new Parallelepiped
                {
                    Center = new(areaX, areaY, areaZ),
                    Height = 2 * areaR,
                    Width = 2 * areaR,
                    Length = 2 * areaR,
                },
                numberOfSeries
            );

            Octree.StartRegionGeneration(maxFullereneSize);

            _areaParams = (new(areaX, areaY, areaZ), areaR);
            _maxRotationAngles = new(maxPracessioAngle, maxNutationAngle, maxProperRotationAngle);
            _fullereneSize = (minFullereneSize, maxFullereneSize);
            _sizeDistr = (shape, scale);
        }

        private (Vector3 center, float r) _areaParams;
        private Vector3 _maxRotationAngles;
        private (float min, float max) _fullereneSize;
        private (float shape, float scale) _sizeDistr;

        public override LimitedArea CreateLimitedArea(int series)
        {
            return new SphereLimitedArea(
                _areaParams.center.X, _areaParams.center.Y, _areaParams.center.Z, _areaParams.r,
                Octree, series,
                CreateFullerene);
        }

        public override Fullerene CreateFullerene()
        {
            return new IcosahedronFullerene(
                _areaParams.center.X - _areaParams.r, _areaParams.center.X + _areaParams.r,
                _areaParams.center.Y - _areaParams.r, _areaParams.center.Y + _areaParams.r,
                _areaParams.center.Z - _areaParams.r, _areaParams.center.Z + _areaParams.r,
                _maxRotationAngles.X, _maxRotationAngles.Y, _maxRotationAngles.Z,
                _fullereneSize.min, _fullereneSize.max,
                _sizeDistr.shape, _sizeDistr.scale);
        }
    }
}
