using System;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Submission
{
    public class SubmissionMetricsService
    {
        private readonly IPaymentLogger logger;

        public SubmissionMetricsService(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task BuildMetrics(long jobId, long ukprn, byte collectionPeriod, short academicYear)
        {

        }
    }
}