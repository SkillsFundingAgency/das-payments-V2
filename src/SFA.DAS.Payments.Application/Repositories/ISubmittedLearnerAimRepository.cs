using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Payments.Application.Repositories
{
    public interface ISubmittedLearnerAimRepository
    {
        Task<int> DeletePreviouslySubmittedAims(byte period, short academicYear, DateTime newIlrSubmissionDate, CancellationToken cancellationToken);
    }

    public class SubmittedLearnerAimRepository : ISubmittedLearnerAimRepository
    {
        private readonly IPaymentsDataContext paymentsDataContext;

        public SubmittedLearnerAimRepository(IPaymentsDataContext paymentsDataContext)
        {
            this.paymentsDataContext = paymentsDataContext;
        }

        public async Task<int> DeletePreviouslySubmittedAims(byte period, short academicYear, DateTime newIlrSubmissionDate, CancellationToken cancellationToken)
        {
            return await paymentsDataContext.Database.ExecuteSqlCommandAsync(@"
                delete from [Payments2].[SubmittedLearnerAim] 
                where 
                    [AcademicYear] = @academicYear 
                    and [CollectionPeriod] = @period 
                    and [IlrSubmissionDateTime] < @newIlrSubmissionDate",
                cancellationToken,
                new {academicYear, period, newIlrSubmissionDate}).ConfigureAwait(false);
        }
    }
}
