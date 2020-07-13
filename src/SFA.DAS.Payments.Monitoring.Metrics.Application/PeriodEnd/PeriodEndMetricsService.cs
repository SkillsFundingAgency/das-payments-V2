using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Metrics.Data;

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
        private readonly IPeriodEndSummaryFactory periodEndSummaryFactory;
        private readonly IDcMetricsDataContext dcDataContext;

        public PeriodEndMetricsService(IPaymentLogger logger, IPeriodEndSummaryFactory periodEndSummaryFactory,IDcMetricsDataContext dcDataContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.periodEndSummaryFactory = periodEndSummaryFactory ?? throw new ArgumentNullException(nameof(periodEndSummaryFactory));
            this.dcDataContext = dcDataContext ?? throw new ArgumentNullException(nameof(dcDataContext));
        }

        public Task BuildMetrics(long jobId, short academicYear, byte collectionPeriod,
            CancellationToken cancellationToken)
        {
            logger.LogDebug($"Building period end metrics for {academicYear}, {collectionPeriod} using job id {jobId}");

            var stopwatch = Stopwatch.StartNew();
            var periodEndSummary = periodEndSummaryFactory.Create(jobId, collectionPeriod, academicYear);

            var dcEarningsTask = dcDataContext.GetEarningsSummary(academicYear, collectionPeriod, cancellationToken);



            stopwatch.Stop();
            logger.LogInfo(
                $"Finished building period end metrics for {academicYear}, {collectionPeriod} using job id {jobId}");
            return Task.CompletedTask;
        }
    }
}