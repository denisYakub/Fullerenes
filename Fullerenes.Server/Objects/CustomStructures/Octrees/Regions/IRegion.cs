using Fullerenes.Server.Objects.Fullerenes;

namespace Fullerenes.Server.Objects.CustomStructures.Octrees.Regions
{
    public interface IRegion
    {
        IRegion[] Split8Parts();
        bool CreateCondition(float maxFigureSize);
        bool Contains(Fullerene fullerene);
        bool ContainsPart(Fullerene fullerene);

    }
}
