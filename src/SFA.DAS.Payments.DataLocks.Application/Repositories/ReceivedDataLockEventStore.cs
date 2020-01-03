using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Repositories
{

    public interface IReceivedDataLockEventStore
    {
        Task Add(ReceivedDataLockEvent dataLock);
        Task<IEnumerable<ReceivedDataLockEvent>> GetDataLocks(long jobId, long ukprn);
        Task Remove(long jobId, long ukprn);
    }

    public class ReceivedDataLockEventStore : IReceivedDataLockEventStore
    {
        private readonly IPaymentsDataContext context;

        public ReceivedDataLockEventStore(IPaymentsDataContext context) 
        {
            this.context = context;
        }

        public async Task Add(ReceivedDataLockEvent dataLock)
        {
            await context.ReceivedDataLockEvents.AddAsync(dataLock);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReceivedDataLockEvent>> GetDataLocks(long jobId, long ukprn)
        {
            var dataLocks = await context.ReceivedDataLockEvents
                .Where(x => x.JobId == jobId && x.Ukprn == ukprn)
                .ToListAsync();
            return dataLocks;
        }

        public async Task Remove(long jobId, long ukprn)
        {
            context.ReceivedDataLockEvents
                .RemoveRange(context.ReceivedDataLockEvents.Where(x => x.JobId == jobId && x.Ukprn == ukprn));
            await context.SaveChangesAsync();
        }
    }
}
