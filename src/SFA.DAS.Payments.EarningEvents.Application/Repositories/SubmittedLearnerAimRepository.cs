using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;

namespace SFA.DAS.Payments.EarningEvents.Application.Repositories
{
    public interface ISubmittedLearnerAimRepository
    {
        Task<int> DeletePreviouslySubmittedAims(long ukprn, byte period, short academicYear, DateTime newIlrSubmissionDate, CancellationToken cancellationToken);
    }

    public class SubmittedLearnerAimRepository : ISubmittedLearnerAimRepository
    {
        private readonly IPaymentsDataContext paymentsDataContext;

        public SubmittedLearnerAimRepository(IPaymentsDataContext paymentsDataContext)
        {
            this.paymentsDataContext = paymentsDataContext;
        }

        public async Task<int> DeletePreviouslySubmittedAims(long ukprn, byte period, short academicYear, DateTime newIlrSubmissionDate, CancellationToken cancellationToken)
        {
            return await paymentsDataContext.Database.ExecuteSqlCommandAsync($@"
                delete from [Payments2].[SubmittedLearnerAim] 
                where 
                    [Ukprn] = {ukprn}
                    and [AcademicYear] = {academicYear}
                    and [CollectionPeriod] = {period}
                    and [IlrSubmissionDateTime] <= {newIlrSubmissionDate}",
                cancellationToken
            ).ConfigureAwait(false);
        }
    }
}
