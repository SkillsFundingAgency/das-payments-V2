using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.PeriodEnd
{
    public interface IPeriodEndMetricsService
    {
        Task BuildMetrics(long jobId, short academicYear, byte collectionPeriod,
            CancellationToken cancellationToken);
    }


    public class PeriodEndMetricsService : IPeriodEndMetricsService
    {
        private readonly IPaymentLogger logger;

        public PeriodEndMetricsService(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task BuildMetrics(long jobId, short academicYear, byte collectionPeriod,
            CancellationToken cancellationToken)
        {
            logger.LogDebug($"Building period end metrics for {academicYear}, {collectionPeriod} using job id {jobId}");

            var stopwatch = Stopwatch.StartNew();


            stopwatch.Stop();
            logger.LogInfo(
                $"Finished building period end metrics for {academicYear}, {collectionPeriod} using job id {jobId}");
            return Task.CompletedTask;
        }
    }
}