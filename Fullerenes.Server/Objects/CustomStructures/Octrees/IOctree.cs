using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Fullerenes;
using Microsoft.Diagnostics.Runtime;

namespace Fullerenes.Server.Objects.CustomStructures.Octree
{
    public interface IOctree
    {
        bool Add(Fullerene fullerene, int thread);
    }
}
