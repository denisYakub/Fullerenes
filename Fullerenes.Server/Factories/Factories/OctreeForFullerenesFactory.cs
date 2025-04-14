using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.CustomStructures.Octree;
using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Fullerenes;
using System.Numerics;

namespace Fullerenes.Server.Factories.Factories
{
    public class OctreeForFullerenesFactory<T> : OctreeAbstractFactory<T> where T : Fullerene
    {
        public override IOctree<T> Generate(IRegion Region, int threadNumber, float maxSize)
        {
            var octree = new Octree<T>(Region, threadNumber);

            octree.StartRegionGeneration(maxSize);

            return octree;
        }
        public override IRegion Generate(Vector3 center, float radius)
        {
            return new Parallelepiped { 
                Center = center, 
                Height = 2 * radius, 
                Length = 2 * radius, 
                Width = 2 * radius 
            };
        }
    }
}

