using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public class ReceivedDataLockEventContext : IReceivedDataLockEventStore
    {
        private readonly IPaymentsDataContext context;
      

        public ReceivedDataLockEventContext(IPaymentsDataContext context) 
        {
            this.context = context;
        }

        public async Task Add(ReceivedDataLockEvent dataLock)
        {
            await context.ReceivedDataLockEvents.AddAsync(dataLock);
            await context.SaveChangesAsync();
        }

        public Task<IEnumerable<ReceivedDataLockEvent>> GetDataLocks(long jobId, long ukprn)
        {
            return Task.FromResult(context.ReceivedDataLockEvents.AsEnumerable());
        }

        public async Task Remove(long jobId, long ukprn)
        {
            context.ReceivedDataLockEvents.RemoveRange(
                context.ReceivedDataLockEvents.Where(x => x.JobId == jobId && x.Ukprn == ukprn));
                context.ReceivedDataLockEvents.Where(x => x.JobId == jobId && x.Ukprn == ukprn));
            await context.SaveChangesAsync();
        }
    }
}
