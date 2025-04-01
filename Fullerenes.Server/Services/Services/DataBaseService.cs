using Fullerenes.Server.DataBase;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using Fullerenes.Server.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Fullerenes.Server.Services.Services
{
    public class DataBaseService(ApplicationDbContext context) : IDataBaseService, IDisposable
    {
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        public async Task<LimitedArea> GetAreaOnlyAsync(int areaId)
        {
            var limitedArea = await context
                .Set<LimitedArea>()
                .Where(la => la.Id == areaId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            ArgumentNullException.ThrowIfNull(limitedArea);

            return limitedArea;
        }

        public async Task<LimitedArea> GetAreaWithFullerenesAsync(int areaId, int seriesFs)
        {
            var limitedArea = await context
                    .Set<LimitedArea>()
                    .Include(la =>
                        la.Fullerenes
                            !.Where(fu => fu.Series == seriesFs))
                    .FirstOrDefaultAsync(la => la.Id == areaId)
                    .ConfigureAwait(false);

            ArgumentNullException.ThrowIfNull(limitedArea);

            return limitedArea;
        }

        public async Task<int> SaveAreaAsync(LimitedArea limitedArea)
        {
            context.LimitedAreas.Add(limitedArea);

            await context.SaveChangesAsync().ConfigureAwait(false);

            ArgumentNullException.ThrowIfNull(limitedArea);

            return limitedArea.Id;
        }

        public void SaveFullerenes(IEnumerable<Fullerene> fullerenes)
        {
            _semaphore.Wait();

            context.AddRange(fullerenes);
            context.SaveChanges();

            _semaphore.Release();
        }

        public async Task<LimitedArea> GetAreaWithFullerenesAsync(int areaId)
        {
            var limitedArea = await context
                .Set<LimitedArea>()
                .Include(la => la.Fullerenes)
                .FirstOrDefaultAsync(la => la.Id == areaId)
                .ConfigureAwait(false);

            ArgumentNullException.ThrowIfNull(limitedArea);
            ArgumentNullException.ThrowIfNull(limitedArea.Fullerenes);

            return limitedArea;
        }

        public async Task<float> GetFullereneMaxSizeAsync(int areaId)
        {
            var size = await context
                .Set<Fullerene>()
                .Where(f => f.LimitedAreaId == areaId)
                .OrderByDescending(f => f.Size)
                .Select(f => f.Size)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return size;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            _semaphore.Dispose();
            context.Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
