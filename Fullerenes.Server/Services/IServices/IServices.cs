using System.Numerics;
using Fullerenes.Server.DataBase;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using Microsoft.AspNetCore.Mvc;
using static Fullerenes.Server.Services.Services.FileService;

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
        (long id, List<long> superIds) GenerateArea(SystemAbstractFactory factory);
        Task<List<float>> GeneratePhis(string dataPath, int numberOfLayers = 5, int numberOfPoints = 1_000_000);
        (IReadOnlyCollection<float> q, IReadOnlyCollection<float> I) GenerateIntensOpt(string dataPath, float qMin, float qMax, int qNum);
    }
    public interface IFileService
    {
        string Write(IReadOnlyCollection<LimitedArea> areas, string fileName, string? subFolder = null);
        AreaMainInfo ReadMainInfo(string fullPath);
        LimitedArea GetArea(string fullPath);
        LimitedArea[] GetAreas(long genId);
    }
}
