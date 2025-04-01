using System.Numerics;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.Enums;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;

namespace Fullerenes.Server.Services.IServices
{
    public interface IFactoryService
    {
        FullereneAndLimitedAreaFactory GetFactory(AreaTypes areaType, FullereneTypes fullereneType, CreateFullerenesAndLimitedAreaRequest request);
    }
    public interface ITestService
    {
        Task<bool> CheckFullerenesIntersectionAsync(IReadOnlyCollection<Fullerene> fullerenes, Vector3 areaCenter, float areaRadius);
    }
    public interface IDataBaseService
    {
        void SaveFullerenes(IEnumerable<Fullerene> fullerenes);
        Task<int> SaveAreaAsync(LimitedArea limitedArea);
        Task<LimitedArea> GetAreaWithFullerenesAsync(int areaId);
        Task<float> GetFullereneMaxSizeAsync(int areaId);
        Task<LimitedArea> GetAreaOnlyAsync(int areaId);
        Task<LimitedArea> GetAreaWithFullerenesAsync(int areaId, int seriesFs);
    }
    public interface ICreateService
    {
        Task<int> GenerateAreaAsync(FullereneAndLimitedAreaFactory factory);
        Task<(float[], float)> GenerateDensityAsync(int areaId, int seriesFs, int numberOfLayers, int numberOfPoints);
    }
}
