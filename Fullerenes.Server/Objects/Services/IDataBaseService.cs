using Fullerenes.Server.DataBase;

namespace Fullerenes.Server.Objects.Services
{
    public interface IDataBaseService
    {
        long GetGenerationId();
        void SaveData(SpData data);
        void SaveGen(SpGen gen);
        string? GetDataPath(long superId);
        ICollection<SpGenGroupView> GetAvgPhiGroups();
    }
}
