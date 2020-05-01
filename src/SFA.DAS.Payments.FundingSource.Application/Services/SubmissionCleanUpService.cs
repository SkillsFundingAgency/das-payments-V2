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

            await dataContext.DeletePreviousSubmissions(commandJobId, collectionPeriod, academicYear,
                commandSubmissionDate, ukprn);

            logger.LogInfo("Finished removing previous submission payments.");
        }

        public async Task RemoveCurrentSubmission(long commandJobId, byte collectionPeriod, short academicYear, long ukprn)
        {
            logger.LogDebug($"Removing current submissions for job id {commandJobId}, " +
                            $"collection period {collectionPeriod}, " +
                            $"academic year {academicYear}, " +
                            $"ukprn {ukprn}");

            await dataContext.DeleteCurrentSubmissions(commandJobId, collectionPeriod, academicYear,ukprn);


            logger.LogInfo("Finished removing previous submission payments.");
        }
    }
}