using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;

namespace SFA.DAS.Payments.Audit.Application.Data.FundingSource
{
    public interface IFundingSourceEventRepository
    {
        Task RemovePriorEvents(long ukprn, short academicYear, byte collectionPeriod, DateTime latestIlrSubmissionTime,
            CancellationToken cancellationToken);

        Task RemoveFailedSubmissionEvents(long jobId, CancellationToken cancellationToken);
    }

    public class FundingSourceEventRepository : IFundingSourceEventRepository
    {
        private readonly IPaymentsDataContext dataContext;

        public FundingSourceEventRepository(IPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task RemovePriorEvents(long ukprn, short academicYear, byte collectionPeriod, DateTime latestIlrSubmissionTime, CancellationToken cancellationToken)
        {
            await dataContext.Database.ExecuteSqlCommandAsync($@"
                    Delete 
                        From [Payments2].[FundingSourceEvent] 
                    Where 
                        Ukprn = {ukprn}
                        And AcademicYear = {academicYear}
                        And CollectionPeriod = {collectionPeriod}
                        And IlrSubmissionDateTime < {latestIlrSubmissionTime}", cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task RemoveFailedSubmissionEvents(long jobId, CancellationToken cancellationToken)
        {
            await dataContext.Database.ExecuteSqlCommandAsync($@"
                    Delete 
                        From [Payments2].[FundingSourceEvent] 
                    Where 
                        JobId = {jobId}", cancellationToken)
                .ConfigureAwait(false);
        }
    }
}