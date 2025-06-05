using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Enums;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using MathNet.Numerics.Distributions;
using System.Numerics;

namespace Fullerenes.Server.Factories.Factories
{
    public class SystemOSIFactory : SystemAbstractFactory
    {
        public override AreaTypes AreaType { get; init; } = AreaTypes.Sphere;
        public override FullereneTypes FullereneType { get; init; } = FullereneTypes.Icosahedron;

        public required CubeRegion StartRegion { get; set; }
        public required Vector3 AreaCenter { get; set; }
        public required float AreaRadius { get; set; }
        public required Vector3 FullereneMinCenter {  get; set; }
        public required Vector3 FullereneMaxCenter { get; set; }
        public required EulerAngles RotationAngles { get; set; }
        public required (float min, float max) FullereneSize { get; set; }
        public required (float shape, float scale) FullereneSizeDistribution { get; set; }
        public override required int FullerenesNumber { get; set; }

        protected override IOctree GenerateOctree()
        {
            return new Octree(StartRegion.MaxDepth(3 * FullereneSize.max), StartRegion);
        }

        protected override Fullerene GenerateFullerene(
            float x, float y, float z,
            float praecessioAngle, float nutatioAngle, float properRotationAngle, 
            float size)
        {
            return new IcosahedronFullerene(
                x, y, z,
                praecessioAngle, nutatioAngle, properRotationAngle,
                size);
        }

        public override LimitedArea GenerateLimitedArea(int thread)
        {
            var area = new SphereLimitedArea(
                AreaCenter.X, AreaCenter.Y, AreaCenter.Z, 
                AreaRadius, 
                thread) 
            {
                ProduceOctree = GenerateOctree, ProduceFullerene = GenerateFullerene,
                Random = new Random(),
                Gamma = new Gamma(FullereneSizeDistribution.shape, FullereneSizeDistribution.scale)
            };

            area.StartGeneration(FullerenesNumber, RotationAngles, FullereneSize);

            return area;
        }
    }
}
