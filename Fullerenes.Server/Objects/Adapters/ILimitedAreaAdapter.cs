using Fullerenes.Server.Objects.LimitedAreas;

namespace Fullerenes.Server.Objects.Adapters
{
    public interface ILimitedAreaAdapter
    {
        string Write(IReadOnlyCollection<LimitedArea> areas, string fileName, string? subFolder = null);
        LimitedArea Read(string fileName, string? subFolder = null);
    }
}
