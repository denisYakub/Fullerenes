using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;

namespace Fullerenes.Server.Objects.CustomStructures.Octree
{
    public interface IOctree<TData>
    {
        void StartRegionGeneration(float maxFigureSize);
        bool AddData(TData inputData, int thread, Func<TData, bool> checkIfDataCannotBeAdded);
        void ClearThreadCollection(int thread);
    }
}
