using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.Enums;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;

namespace Fullerenes.Server.Factories.AbstractFactories
{
    public abstract class SystemAbstractFactory
    {
        public abstract AreaTypes AreaType { get; init; }
        public abstract FullereneTypes FullereneType { get; init; }
        public abstract required int ThreadNumber {  get; set; }
        public abstract required int FullerenesNumber { get; set; }
        public abstract IOctree GenerateOctree();
        public abstract Fullerene GenerateFullerene(
            float x, float y, float z,
            float praecessioAngle, float nutatioAngle, float properRotationAngle,
            float size);
        public abstract LimitedArea GenerateLimitedArea(int thread, IOctree octree);
    }
}
