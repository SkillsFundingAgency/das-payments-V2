using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Application.Data;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public class SubmissionCleanUpService : ISubmissionCleanUpService
    {
        private readonly IPaymentLogger logger;
        private readonly IFundingSourceDataContext dataContext;

        public SubmissionCleanUpService(IPaymentLogger logger, IFundingSourceDataContext dataContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task RemovePreviousSubmissions(long commandJobId, byte collectionPeriod, short academicYear,
            DateTime commandSubmissionDate, long ukprn)
        {
            logger.LogDebug($"Removing previous submissions for job id {commandJobId}, " +
                            $"collection period {collectionPeriod}, " +
                            $"academic year {academicYear}, " +
                            $"ukprn {ukprn}");

            //dataContext.LevyTransactions.RemoveRange(dataContext.LevyTransactions.Where(transaction =>
            //    transaction.AcademicYear == academicYear
            //    && transaction.CollectionPeriod == collectionPeriod
            //    && transaction.JobId != commandJobId
            //    //todo ilrSubmissionDateTime
            //    && transaction.Ukprn == ukprn));

            //await dataContext.SaveChanges(CancellationToken.None);
            await dataContext.DeletePreviousSubmissions(commandJobId, collectionPeriod, academicYear,
                commandSubmissionDate, ukprn);

            logger.LogInfo("Finished removing previous submission payments.");
        }

        public async Task RemoveCurrentSubmission(long commandJobId, byte collectionPeriod, short academicYear,
            DateTime commandSubmissionDate, long ukprn)
        {
            //do nothing todo implement this
        }
    }
}