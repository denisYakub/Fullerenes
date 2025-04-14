using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using System.Numerics;

namespace Fullerenes.Server.Factories.AbstractFactories
{
    public abstract class OctreeAbstractFactory<T>
    {
        public abstract IOctree<T> Generate(IRegion Region, int threadNumber, float maxSize);
        public abstract IRegion Generate(Vector3 center, float radius);
    }
}
