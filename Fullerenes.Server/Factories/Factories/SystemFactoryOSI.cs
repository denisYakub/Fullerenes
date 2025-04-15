using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.Adapters;
using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Enums;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using System.Numerics;

namespace Fullerenes.Server.Factories.Factories
{
    public class SystemFactoryOSI<TRegion>(ILimitedAreaAdapter adapter) 
        : SystemAbstractFactory(adapter) where TRegion : IRegion
    {
        public override AreaTypes AreaType { get; init; } = AreaTypes.Sphere;
        public override FullereneTypes FullereneType { get; init; } = FullereneTypes.Icosahedron;

        public required TRegion StartRegion { get; set; }
        public override required int ThreadNumber { get; set; }
        public required Vector3 AreaCenter { get; set; }
        public required float AreaRadius { get; set; }
        public required Vector3 FullereneMinCenter {  get; set; }
        public required Vector3 FullereneMaxCenter { get; set; }
        public required EulerAngles RotationAngles { get; set; }
        public required (float min, float max) FullereneSize { get; set; }
        public required (float shape, float scale) FullereneSizeDistribution { get; set; }
        public override required int FullerenesNumber { get; set; }

        public override IOctree<Fullerene> GenerateOctree()
        {
            Octree<Fullerene> octree = new(StartRegion, ThreadNumber);

            octree.StartRegionGeneration(FullereneSize.max);

            return octree;
        }

        public override Fullerene GenerateFullerene()
        {
            return new IcosahedronFullerene(
                FullereneMinCenter.X, FullereneMaxCenter.X,
                FullereneMinCenter.Y, FullereneMaxCenter.Y,
                FullereneMinCenter.Z, FullereneMaxCenter.Z,
                RotationAngles.PraecessioAngle, 
                RotationAngles.NutatioAngle, 
                RotationAngles.ProperRotationAngle,
                FullereneSize.min, FullereneSize.max,
                FullereneSizeDistribution.shape, FullereneSizeDistribution.scale);
        }

        public override LimitedArea GenerateLimitedArea(int thread, IOctree<Fullerene> octree)
        {
            var area = new SphereLimitedArea(
                AreaCenter.X, AreaCenter.Y, AreaCenter.Z, 
                AreaRadius, 
                thread) 
            { Octree = octree, ProduceFullerene = GenerateFullerene };

            area.StartGeneration(FullerenesNumber);

            return area;
        }
    }
}
