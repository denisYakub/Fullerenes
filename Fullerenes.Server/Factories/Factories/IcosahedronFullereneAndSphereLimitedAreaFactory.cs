using CsvHelper;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.Enums;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using System.Numerics;
using static MessagePack.GeneratedMessagePackResolver.Fullerenes.Server.Objects;

namespace Fullerenes.Server.Factories.Factories
{
    public class IcosahedronFullereneAndSphereLimitedAreaFactory : FullereneAndLimitedAreaFactory
    {
        private Vector3 _areaCenter;
        private float _areaRaduis;
        private Vector3 _maxRotationAngles;
        private (float min, float max) _fullereneSize;
        private (float shape, float scale) _sizeDistr;
        public IcosahedronFullereneAndSphereLimitedAreaFactory(
            float areaX, float areaY, float areaZ, float areaR,
            int numberOfSeries, int numberOfFullerenes,
            float maxPracessioAngle, float maxNutationAngle, float maxProperRotationAngle,
            float minFullereneSize, float maxFullereneSize,
            float shape, float scale)
            : base(
                  numberOfSeries, numberOfFullerenes,
                  minFullereneSize, maxFullereneSize,
                  AreaTypes.Sphere, FullereneTypes.Icosahedron)
        {
            Octree = new Octree<Parallelepiped, Fullerene>(
                new Parallelepiped
                {
                    Center = new(areaX, areaY, areaZ),
                    Height = 2 * areaR,
                    Width = 2 * areaR,
                    Length = 2 * areaR,
                },
                numberOfSeries
            );

            Octree.GenerateRegions(Parallelepiped.Split8Parts, p => p.Width > 3 * maxFullereneSize);

            _areaCenter = new(areaX, areaY, areaZ);
            _areaRaduis = areaR;
            _maxRotationAngles = new(maxPracessioAngle, maxNutationAngle, maxProperRotationAngle);
            _fullereneSize = (minFullereneSize, maxFullereneSize);
            _sizeDistr = (shape, scale);
        }

        public override LimitedArea CreateLimitedArea(int series)
        {
            return new SphereLimitedArea(
                _areaCenter.X, _areaCenter.Y, _areaCenter.Z, _areaRaduis,
                Octree, series,
                CreateFullerene);
        }

        public override Fullerene CreateFullerene()
        {
            return new IcosahedronFullerene(
                _areaCenter.X - _areaRaduis, _areaCenter.X + _areaRaduis,
                _areaCenter.Y - _areaRaduis, _areaCenter.Y + _areaRaduis,
                _areaCenter.Z - _areaRaduis, _areaCenter.Z + _areaRaduis,
                _maxRotationAngles.X, _maxRotationAngles.Y, _maxRotationAngles.Z,
                _fullereneSize.min, _fullereneSize.max,
                _sizeDistr.shape, _sizeDistr.scale);
        }
    }
}
