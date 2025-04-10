using System.Numerics;
using Fullerenes.Server.DataBase;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.Dtos;
using Fullerenes.Server.Objects.Enums;
using Fullerenes.Server.Objects.Fullerenes;

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
        long GetGenerationId();
        void SaveData(SpData data);
        void SaveGen(SpGen gen);
    }
    public interface ICreateService
    {
        long GenerateArea(FullereneAndLimitedAreaFactory factory);
    }
}
