using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Payments.Application.Repositories
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
            return await paymentsDataContext.Database.ExecuteSqlCommandAsync(@"
                delete from [Payments2].[SubmittedLearnerAim] 
                where 
                    [Ukprn] = {0}
                    and [AcademicYear] = {1}
                    and [CollectionPeriod] = {2}
                    and [IlrSubmissionDateTime] <= {3}",
                new object[] {ukprn, academicYear, period, newIlrSubmissionDate},
                cancellationToken
            ).ConfigureAwait(false);
        }
    }
}
