using Fullerenes.Server.Objects.LimitedAreas;

namespace Fullerenes.Server.Objects.Adapters
{
    public interface ILimitedAreaAdapter
    {
        string Write(IReadOnlyCollection<LimitedArea> areas, string fileName);
        LimitedArea Read(int series, string fileName);
    }
}
