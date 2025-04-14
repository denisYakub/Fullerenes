using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Fullerenes;

namespace Fullerenes.Server.Objects.CustomStructures.Octree
{
    public interface IOctree<TData>
    {
        void StartRegionGeneration(float maxFigureSize);
        bool AddData(TData inputData, int thread, Func<TData, bool> checkIfDataCannotBeAdded);
        void ClearThreadCollection(int thread);
    }
}
