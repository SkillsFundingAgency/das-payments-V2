using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;

namespace SFA.DAS.Payments.Audit.Application.Data.RequiredPayment
{
    public interface IRequiredPaymentEventRepository
    {
        Task RemovePriorEvents(long ukprn, short academicYear, byte collectionPeriod, DateTime latestIlrSubmissionTime,
            CancellationToken cancellationToken);

        Task RemoveFailedSubmissionEvents(long jobId, CancellationToken cancellationToken);
    }

    public class RequiredPaymentEventRepository : IRequiredPaymentEventRepository
    {
        private readonly IPaymentsDataContext dataContext;

        public RequiredPaymentEventRepository(IPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task RemovePriorEvents(long ukprn, short academicYear, byte collectionPeriod, DateTime latestIlrSubmissionTime, CancellationToken cancellationToken)
        {
            await dataContext.Database.ExecuteSqlCommandAsync($@"
                    Delete 
                        From [Payments2].[RequiredPaymentEvent] 
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
                        From [Payments2].[RequiredPaymentEvent] 
                    Where 
                        JobId = {jobId}", cancellationToken)
                .ConfigureAwait(false);
        }
    }
}