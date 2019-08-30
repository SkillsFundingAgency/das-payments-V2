using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Application.Repositories
{
    public interface IPeriodEndEventRepository
    {
        Task<PeriodEndEventModel> GetLastPeriodEndEvent(CancellationToken cancellationToken);
        Task RecordPeriodEndEvent(PeriodEndEventModel periodEndEvent, CancellationToken cancellationToken);
    }

    public class PeriodEndEventRepository : IPeriodEndEventRepository
    {
        private readonly IPaymentsDataContext dataContext;

        public PeriodEndEventRepository(IPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<PeriodEndEventModel> GetLastPeriodEndEvent(CancellationToken cancellationToken)
        {
            return await dataContext.PeriodEndEvent
                .OrderByDescending(e => e.EventTime)
                .LastOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task RecordPeriodEndEvent(PeriodEndEventModel periodEndEvent, CancellationToken cancellationToken)
        {
            await dataContext.PeriodEndEvent.AddAsync(periodEndEvent, cancellationToken).ConfigureAwait(false);
            await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
