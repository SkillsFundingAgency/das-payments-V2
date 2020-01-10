using System;
using System.Diagnostics;
using System.Threading;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using System.Threading.Tasks;
using SFA.DAS.Payments.Monitoring.Metrics.Data;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Submission
{
    public interface ISubmissionMetricsService
    {
        Task BuildMetrics(long ukprn, long jobId, short academicYear, byte collectionPeriod,
            CancellationToken cancellationToken);
    }

    public class SubmissionMetricsService : ISubmissionMetricsService
    {
        private readonly IPaymentLogger logger;
        private readonly ISubmissionSummaryFactory submissionSummaryFactory;
        private readonly IDcMetricsDataContext dcDataContext;
        private readonly ISubmissionMetricsRepository submissionRepository;

        public SubmissionMetricsService(IPaymentLogger logger, ISubmissionSummaryFactory submissionSummaryFactory, IDcMetricsDataContext dcDataContext, ISubmissionMetricsRepository submissionRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.submissionSummaryFactory = submissionSummaryFactory ?? throw new ArgumentNullException(nameof(submissionSummaryFactory));
            this.dcDataContext = dcDataContext ?? throw new ArgumentNullException(nameof(dcDataContext));
            this.submissionRepository = submissionRepository ?? throw new ArgumentNullException(nameof(submissionRepository));
        }

        public async Task BuildMetrics(long ukprn, long jobId, short academicYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogDebug($"Building metrics for job: {jobId}, provider: {ukprn}, Academic year: {academicYear}, Collection period: {collectionPeriod}");
                var stopwatch = Stopwatch.StartNew();
                var submissionSummary = submissionSummaryFactory.Create(ukprn, jobId, academicYear, collectionPeriod);
                var dcEarningsTask = dcDataContext.GetEarnings(ukprn, academicYear, collectionPeriod);
                var dasEarningsTask = submissionRepository.GetDasEarnings(ukprn, jobId);
                var dataLocksTask = submissionRepository.GetDataLockedEarnings(ukprn, jobId);
                var dataLocksTotalTask = submissionRepository.GetDataLockedEarningsTotal(ukprn, jobId);
                var requiredPaymentsTask = submissionRepository.GetRequiredPayments(ukprn, jobId);
                var heldBackCompletionAmountsTask = submissionRepository.GetHeldBackCompletionPaymentsTotal(ukprn, jobId);
                await Task.WhenAll(dcEarningsTask, dasEarningsTask, dataLocksTask, dataLocksTotalTask, requiredPaymentsTask, heldBackCompletionAmountsTask).ConfigureAwait(false);
                stopwatch.Stop();
                logger.LogDebug($"finished getting data from databases. Took: {stopwatch.ElapsedMilliseconds}ms for metrics report for job: {jobId}, ukprn: {ukprn}");
                submissionSummary.AddEarnings(dcEarningsTask.Result, dasEarningsTask.Result);
                submissionSummary.AddDataLockedEarnings(dataLocksTotalTask.Result, dataLocksTask.Result);
                submissionSummary.AddRequiredPayments(requiredPaymentsTask.Result);
                submissionSummary.AddHeldBackCompletionPayments(heldBackCompletionAmountsTask.Result);
                var metrics = submissionSummary.GetMetrics();
                await submissionRepository.SaveSubmissionMetrics(metrics, cancellationToken);
                logger.LogInfo($"Finished building metrics for job: {jobId}, provider: {ukprn}, Academic year: {academicYear}, Collection period: {collectionPeriod}");
            }
            catch (Exception e)
            {
                logger.LogWarning($"Error building the submission metrics report for job: {jobId}, ukprn: {ukprn}. Error: {e}");
                throw;
            }
        }
    }
}