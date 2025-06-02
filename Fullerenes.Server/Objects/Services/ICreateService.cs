using Fullerenes.Server.Factories.AbstractFactories;

namespace Fullerenes.Server.Objects.Services
{
    public interface ICreateService
    {
        (long id, List<long> superIds) GenerateArea(SystemAbstractFactory factory, int series);
        Task<List<float>> GeneratePhis(string dataPath, int numberOfLayers = 5, int numberOfPoints = 1_000_000);
        (IReadOnlyCollection<float> q, IReadOnlyCollection<float> I) GenerateIntensOpt(string dataPath, float qMin, float qMax, int qNum);
    }
}
