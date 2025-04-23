using System.Numerics;
using Fullerenes.Server.DataBase;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.Fullerenes;

namespace Fullerenes.Server.Services.IServices
{
    public interface ITestService
    {
        Task<bool> CheckFullerenesIntersectionAsync(IReadOnlyCollection<Fullerene> fullerenes, Vector3 areaCenter, float areaRadius);
    }
    public interface IDataBaseService
    {
        long GetGenerationId();
        void SaveData(SpData data);
        void SaveGen(SpGen gen);
        string? GetDataPath(long superId);
        ICollection<SpGenGroup> GetAvgPhiGroups();
    }
    public interface ICreateService
    {
        long GenerateArea(SystemAbstractFactory factory);
        Task<float[]> GeneratePhis(long superId, int numberOfLayers = 5, int numberOfPoints = 1_000_000);
    }
}
