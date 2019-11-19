using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    public class ReceivedDataLockEventContext : DbContext, IReceivedDataLockEventStore
    {
        public DbSet<ReceivedDataLockEvent> DataLockEvents { get; set; }

        public ReceivedDataLockEventContext(DbContextOptions options) : base(options)
        {
        }

        public async Task Add(ReceivedDataLockEvent dataLock)
        {
            await DataLockEvents.AddAsync(dataLock);
            await SaveChangesAsync();
        }

        public Task<IEnumerable<ReceivedDataLockEvent>> GetDataLocks(long jobId, long ukprn)
        {
            return Task.FromResult(DataLockEvents.AsEnumerable());
        }

        public async Task Remove(long jobId, long ukprn)
        {
            DataLockEvents.RemoveRange(
                DataLockEvents.Where(x => x.JobId == jobId && x.Ukprn == ukprn));
            await SaveChangesAsync();
        }
    }
}
