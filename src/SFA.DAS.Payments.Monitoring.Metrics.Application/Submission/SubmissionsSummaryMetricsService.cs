using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Submission
{
    public interface ISubmissionsSummaryMetricsService
    {
        Task GenrateSubmissionsSummaryMetrics(long jobId, short academicYear, byte collectionPeriod, CancellationToken cancellationToken);
    }

    public class SubmissionsSummaryMetricsService : ISubmissionsSummaryMetricsService
    {
        private readonly IPaymentLogger logger;
        private readonly ISubmissionMetricsRepository submissionMetricsRepository;
        private readonly ITelemetry telemetry;

        public SubmissionsSummaryMetricsService(IPaymentLogger logger, ISubmissionMetricsRepository submissionMetricsRepository, ITelemetry telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.submissionMetricsRepository = submissionMetricsRepository ?? throw new ArgumentNullException(nameof(submissionMetricsRepository));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task GenrateSubmissionsSummaryMetrics(long jobId, short academicYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogDebug($"Building metrics for job: {jobId}, Academic year: {academicYear}, Collection period: {collectionPeriod}");

                var stopwatch = Stopwatch.StartNew();

                var metrics = await submissionMetricsRepository.GetSubmissionsSummaryMetrics(jobId, academicYear, collectionPeriod, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                var dataDuration = stopwatch.ElapsedMilliseconds;

                logger.LogDebug($"finished getting data from databases for job: {jobId}, Took: {dataDuration}ms.");

                await submissionMetricsRepository.SaveSubmissionsSummaryMetrics(metrics, cancellationToken);
                
                stopwatch.Stop();

                SendTelemetry(metrics, stopwatch.ElapsedMilliseconds);

                logger.LogInfo($"Finished building Submissions Summary Metrics for job: {jobId}, Academic year: {academicYear}, Collection period: {collectionPeriod}. Took: {stopwatch.ElapsedMilliseconds}ms");
            }
            catch (Exception e)
            {
                logger.LogWarning($"Error building the Submissions Summary metrics report for job: {jobId}, Error: {e}");
                throw;
            }
        }

        private void SendTelemetry(SubmissionsSummaryModel metrics, long reportGenerationDuration)
        {
            var properties = new Dictionary<string, string>
            {
                { TelemetryKeys.JobId, metrics.JobId.ToString()},
                { TelemetryKeys.CollectionPeriod, metrics.CollectionPeriod.ToString()},
                { TelemetryKeys.AcademicYear, metrics.AcademicYear.ToString()},
            };

            var submissionMetrics = metrics.SubmissionMetrics;
            var dasEarnings = metrics.DasEarnings;
            var dcEarnings = metrics.DcEarnings;

            var stats = new Dictionary<string, double>
            {
                { "ReportGenerationDuration", reportGenerationDuration },
                
                { "Percentage" ,                              (double) submissionMetrics.Percentage },
                { "ContractType1Percentage" ,                 (double) submissionMetrics.PercentageContractType1 },
                { "ContractType2Percentage" ,                 (double) submissionMetrics.PercentageContractType2 },
                                                              
                { "DifferenceTotal" ,                         (double) submissionMetrics.DifferenceTotal },
                { "DifferenceContractType1" ,                 (double) submissionMetrics.DifferenceContractType1 },
                { "DifferenceContractType2" ,                 (double) submissionMetrics.DifferenceContractType2 },
                                                              
                { "DasEarningsPercentage" ,                   (double) dasEarnings.Percentage },
                { "DasEarningsPercentageContractType1" ,      (double) dasEarnings.PercentageContractType1 },
                { "DasEarningsPercentageContractType2" ,      (double) dasEarnings.PercentageContractType2 },
                                                              
                { "DasEarningsDifferenceTotal" ,              (double) dasEarnings.DifferenceTotal },
                { "DasEarningsDifferenceContractType1" ,      (double) dasEarnings.DifferenceContractType1 },
                { "DasEarningsDifferenceContractType2" ,      (double) dasEarnings.DifferenceContractType2 },

                { "DasEarningsTotal" ,                        (double) dasEarnings.Total },
                { "DasEarningsContractType1Total" ,           (double) dasEarnings.ContractType1 },
                { "DasEarningsContractType2Total" ,           (double) dasEarnings.ContractType2 },
                                                              
                { "DcEarningsTotal" ,                         (double) dcEarnings.Total },
                { "DcEarningsContractType1Total" ,            (double) dcEarnings.ContractType1 },
                { "DcEarningsContractType2Total" ,            (double) dcEarnings.ContractType2 },
                
                { "DataLockedEarningsAmount" ,                (double) metrics.AdjustedDataLockedEarnings },
                
                { "DataLockedEarningsTotal" ,                 (double) metrics.TotalDataLockedEarnings },
                
                { "DataLockAmountAlreadyPaid" ,               (double) metrics.AlreadyPaidDataLockedEarnings },
                
                { "HeldBackCompletionPayments" ,              (double) metrics.HeldBackCompletionPayments.Total },
                { "HeldBackCompletionPaymentsContractType1" , (double) metrics.HeldBackCompletionPayments.ContractType1 },
                { "HeldBackCompletionPaymentsContractType2" , (double) metrics.HeldBackCompletionPayments.ContractType1 },

                { "YearToDatePaymentsTotal" ,                 (double) metrics.YearToDatePayments.Total },
                { "YearToDatePaymentsContractType1Total",     (double) metrics.YearToDatePayments.ContractType1 },
                { "YearToDatePaymentsContractType2Total",     (double) metrics.YearToDatePayments.ContractType2 },
                
                { "RequiredPaymentsTotal" ,                   (double) metrics.RequiredPayments.Total },
                { "RequiredPaymentsAct1Total" ,               (double) metrics.RequiredPayments.ContractType1 },
                { "RequiredPaymentsAc2Total" ,                (double) metrics.RequiredPayments.ContractType2 },
                
                { "RequiredPaymentsDasEarningsPercentageComparison" ,  Math.Round(((double) (metrics.YearToDatePayments.Total + metrics.RequiredPayments.Total) / (double) metrics.DasEarnings.Total) * 100, 2) }
            };

            telemetry.TrackEvent("Finished Generating Submissions Summary Metrics", properties, stats);
        }
    }
}