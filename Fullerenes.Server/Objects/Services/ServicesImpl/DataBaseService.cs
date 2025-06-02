using System.Runtime.InteropServices;
using Fullerenes.Server.DataBase;

namespace Fullerenes.Server.Objects.Services.ServicesImpl
{
    public class DataBaseService(ApplicationDbContext context) : IDataBaseService, IDisposable
    {
        private bool isDisposed;
        private readonly SemaphoreSlim _semaphore = new(1);

        public long GetGenerationId()
        {
            var counter = context
                .SpGenIdCounter
                .OrderByDescending(sp => sp.Id)
                .FirstOrDefault();

            if (counter is null)
            {
                counter = new SpGenIdCounter();
                context.SpGenIdCounter.Add(counter);
            }
            else
            {
                counter.GenIdCurrent++;
            }

            context.SaveChanges();

            return counter.GenIdCurrent;
        }


        public void SaveData(SpData data)
        {
            _semaphore.Wait();

            context.SpData.Add(data);
            context.SaveChanges();

            _semaphore.Release();
        }

        public void SaveGen(SpGen gen)
        {
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

        public ICollection<SpGenGroupView> GetAvgPhiGroups()
        {
            var viewresult = context.SpGenGroupView.ToList();

            return viewresult;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing) 
                _semaphore.Dispose();

            isDisposed = true;
        }

        ~DataBaseService() => Dispose(false);
    }
}
