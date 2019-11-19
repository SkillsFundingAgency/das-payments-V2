using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    public class ReceivedDataLockEventContext : DbContext, IReceivedDataLockEventStore
    {
        public DbSet<ReceivedDataLockEvent> DataLockEvents { get; set; }

        public ReceivedDataLockEventContext(DbContextOptions options) : base(options)
        {
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<ReceivedDataLockEvent>().HasKey(o => o.Id);
        //}

        public void Add(ReceivedDataLockEvent dataLock)
        {
            DataLockEvents.Add(dataLock);
            SaveChanges();
        }

        public IEnumerable<ReceivedDataLockEvent> GetDataLocks(long jobId, long ukprn)
        {
            return DataLockEvents;
        }

        public void Remove(long jobId, long ukprn)
        {
            DataLockEvents.RemoveRange(
                DataLockEvents.Where(x => x.JobId == jobId && x.Ukprn == ukprn));
            SaveChanges();
        }
    }
}
