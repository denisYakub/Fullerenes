using Fullerenes.Server.DataBase;
using Fullerenes.Server.Services.IServices;

namespace Fullerenes.Server.Services.Services
{
    public class DataBaseService(ApplicationDbContext context) : IDataBaseService//, IDisposable
    {
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        public long GetGenerationId()
        {
            var lastId = context
                .SpGen
                .OrderByDescending(sp => sp.Id)
                .Select(sp => sp.N)
                .FirstOrDefault();

            return ++lastId;
        }

        public void SaveData(SpData data)
        {
            ArgumentNullException.ThrowIfNull(data);

            _semaphore.Wait();

            context.SpData.Add(data);
            context.SaveChanges();

            _semaphore.Release();
        }

        public void SaveGen(SpGen gen)
        {
            ArgumentNullException.ThrowIfNull(gen);

            _semaphore.Wait();

            context.SpGen.Add(gen);
            context.SaveChanges();

            _semaphore.Release();
        }

        public string? GetDataPath(long superId)
        {
            return context
                .SpData
                .Find(superId)
                ?.FilePath;
       }
    }
}
