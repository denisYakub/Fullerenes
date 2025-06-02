using Fullerenes.Server.Objects.Fullerenes;
using System.Numerics;

namespace Fullerenes.Server.Objects.Services
{
    public interface ITestService
    {
        Task<bool> CheckFullerenesIntersectionAsync(IReadOnlyCollection<Fullerene> fullerenes, Vector3 areaCenter, float areaRadius);
    }
}
