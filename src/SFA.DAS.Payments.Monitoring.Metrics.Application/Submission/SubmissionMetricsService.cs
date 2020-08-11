using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Metrics.Data;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

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
        private readonly IDcMetricsDataContextFactory dcMetricsDataContextFactory;
        private readonly ISubmissionMetricsRepository submissionRepository;
        private readonly ITelemetry telemetry;

        public SubmissionMetricsService(IPaymentLogger logger, ISubmissionSummaryFactory submissionSummaryFactory,
            IDcMetricsDataContextFactory dcMetricsDataContextFactory, ISubmissionMetricsRepository submissionRepository, ITelemetry telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.submissionSummaryFactory = submissionSummaryFactory ?? throw new ArgumentNullException(nameof(submissionSummaryFactory));
            this.dcMetricsDataContextFactory = dcMetricsDataContextFactory ?? throw new ArgumentNullException(nameof(dcMetricsDataContextFactory));
            this.submissionRepository = submissionRepository ?? throw new ArgumentNullException(nameof(submissionRepository));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task BuildMetrics(long ukprn, long jobId, short academicYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogDebug($"Building metrics for job: {jobId}, provider: {ukprn}, Academic year: {academicYear}, Collection period: {collectionPeriod}");
                var stopwatch = Stopwatch.StartNew();
                var submissionSummary = submissionSummaryFactory.Create(ukprn, jobId, academicYear, collectionPeriod);
                var dcEarningsTask = dcMetricsDataContextFactory.Get(academicYear).GetEarnings(ukprn, academicYear, collectionPeriod, cancellationToken);
                var dasEarningsTask = submissionRepository.GetDasEarnings(ukprn, jobId, cancellationToken);
                var dataLocksTask = submissionRepository.GetDataLockedEarnings(ukprn, jobId, cancellationToken);
                var dataLocksTotalTask = submissionRepository.GetDataLockedEarningsTotal(ukprn, jobId, cancellationToken);
                var dataLocksAlreadyPaid =
                    submissionRepository.GetAlreadyPaidDataLockedEarnings(ukprn, jobId, cancellationToken);
                var requiredPaymentsTask = submissionRepository.GetRequiredPayments(ukprn, jobId, cancellationToken);
                var heldBackCompletionAmountsTask = submissionRepository.GetHeldBackCompletionPaymentsTotal(ukprn, jobId, cancellationToken);
                var yearToDateAmountsTask = submissionRepository.GetYearToDatePaymentsTotal(ukprn, academicYear, collectionPeriod, cancellationToken);
                var dataTask = Task.WhenAll(dcEarningsTask, dasEarningsTask, dataLocksTask, dataLocksTotalTask, dataLocksAlreadyPaid, requiredPaymentsTask, heldBackCompletionAmountsTask, yearToDateAmountsTask);
                var waitTask = Task.Delay(TimeSpan.FromSeconds(270), cancellationToken);
                Task.WaitAny(dataTask, waitTask);
                cancellationToken.ThrowIfCancellationRequested();
                if (!dataTask.IsCompleted)
                    throw new InvalidOperationException($"Took too long to get data for the submission metrics. Ukprn: {ukprn}, job: {jobId}, Collection period: {collectionPeriod}");
                var dataDuration = stopwatch.ElapsedMilliseconds;
                logger.LogDebug($"finished getting data from databases for job: {jobId}, ukprn: {ukprn}. Took: {dataDuration}ms.");
                submissionSummary.AddEarnings(dcEarningsTask.Result, dasEarningsTask.Result);
                submissionSummary.AddDataLockTypeCounts(dataLocksTotalTask.Result, dataLocksTask.Result, dataLocksAlreadyPaid.Result);
                submissionSummary.AddRequiredPayments(requiredPaymentsTask.Result);
                submissionSummary.AddHeldBackCompletionPayments(heldBackCompletionAmountsTask.Result);
                submissionSummary.AddYearToDatePaymentTotals(yearToDateAmountsTask.Result);
                var metrics = submissionSummary.GetMetrics();
                await submissionRepository.SaveSubmissionMetrics(metrics, cancellationToken);
                stopwatch.Stop();
                SendMetricsTelemetry(metrics, stopwatch.ElapsedMilliseconds);
                logger.LogInfo($"Finished building metrics for submission job: {jobId}, provider: {ukprn}, Academic year: {academicYear}, Collection period: {collectionPeriod}. Took: {stopwatch.ElapsedMilliseconds}ms");
            }
            catch (Exception e)
            {
                logger.LogWarning($"Error building the submission metrics report for job: {jobId}, ukprn: {ukprn}. Error: {e}");
                throw;
            }
        }

        private void SendMetricsTelemetry(SubmissionSummaryModel metrics, long reportGenerationDuration)
        {
            var properties = new Dictionary<string, string>
            {
                { TelemetryKeys.JobId, metrics.JobId.ToString()},
                { TelemetryKeys.Ukprn, metrics.Ukprn.ToString()},
                { TelemetryKeys.CollectionPeriod, metrics.CollectionPeriod.ToString()},
                { TelemetryKeys.AcademicYear, metrics.AcademicYear.ToString()},
            };

            var stats = new Dictionary<string, double>
            {
                { "ReportGenerationDuration", reportGenerationDuration },
                { "Percentage" , (double)metrics.Percentage },
                { "ContractType1Percentage" , (double)metrics.SubmissionMetrics.PercentageContractType1 },
                { "ContractType2Percentage" , (double)metrics.SubmissionMetrics.PercentageContractType2 },
                { "EarningsContractType1Percentage" , (double)metrics.SubmissionMetrics.PercentageContractType1 },
                { "EarningsContractType2Percentage" , (double)metrics.SubmissionMetrics.PercentageContractType2 },
            };

            telemetry.TrackEvent("Finished Generating Submission Metrics", properties, stats);
        }
    }
}