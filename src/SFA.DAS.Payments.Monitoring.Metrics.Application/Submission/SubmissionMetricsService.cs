using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private readonly IDcMetricsDataContext dcDataContext;
        private readonly ISubmissionMetricsRepository submissionRepository;
        private readonly ITelemetry telemetry;

        public SubmissionMetricsService(IPaymentLogger logger, ISubmissionSummaryFactory submissionSummaryFactory,
            IDcMetricsDataContext dcDataContext, ISubmissionMetricsRepository submissionRepository, ITelemetry telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.submissionSummaryFactory = submissionSummaryFactory ?? throw new ArgumentNullException(nameof(submissionSummaryFactory));
            this.dcDataContext = dcDataContext ?? throw new ArgumentNullException(nameof(dcDataContext));
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
                var dcEarningsTask = dcDataContext.GetEarnings(ukprn, academicYear, collectionPeriod, cancellationToken);
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

            var submissionMetrics = metrics.SubmissionMetrics;
            var earningsMetrics = metrics.EarningsMetrics;
            var dataLockMetrics = metrics.DataLockMetrics;
            var requiredPaymentsMetrics = metrics.RequiredPaymentsMetrics;

            var stats = new Dictionary<string, double>
            {
                { "ReportGenerationDuration", reportGenerationDuration },
                { "Percentage" , (double)metrics.Percentage },
                { "ContractType1Percentage" , (double)submissionMetrics.PercentageContractType1 },
                { "ContractType2Percentage" , (double)submissionMetrics.PercentageContractType2 },
                { "EarningsContractType1Percentage" , (double)submissionMetrics.PercentageContractType1 },
                { "EarningsContractType2Percentage" , (double)submissionMetrics.PercentageContractType2 },
                { "DcEarningsTotal" , (double) metrics.DcEarnings.Total },
                { "DasEarningsTotal" , (double) metrics.DasEarnings.Total },
                { "DasEarningsTransactionType1" , (double) earningsMetrics.Sum(x=>x.Amounts.TransactionType1) },
                { "DasEarningsTransactionType2" , (double) earningsMetrics.Sum(x=>x.Amounts.TransactionType2) },
                { "DasEarningsTransactionType3" , (double) earningsMetrics.Sum(x=>x.Amounts.TransactionType3) },
                { "DasEarningsTransactionType4" , (double) earningsMetrics.Sum(x=>x.Amounts.TransactionType4) },
                { "DasEarningsTransactionType5" , (double) earningsMetrics.Sum(x=>x.Amounts.TransactionType5) },
                { "DasEarningsTransactionType6" , (double) earningsMetrics.Sum(x=>x.Amounts.TransactionType6) },
                { "DasEarningsTransactionType7" , (double) earningsMetrics.Sum(x=>x.Amounts.TransactionType7) },
                { "DasEarningsTransactionType8" , (double) earningsMetrics.Sum(x=>x.Amounts.TransactionType8) },
                { "DasEarningsTransactionType9" , (double) earningsMetrics.Sum(x=>x.Amounts.TransactionType9) },
                { "DasEarningsTransactionType10" , (double) earningsMetrics.Sum(x=>x.Amounts.TransactionType10) },
                { "DasEarningsTransactionType11" , (double) earningsMetrics.Sum(x=>x.Amounts.TransactionType11) },
                { "DasEarningsTransactionType12" , (double) earningsMetrics.Sum(x=>x.Amounts.TransactionType12) },
                { "DasEarningsTransactionType13" , (double) earningsMetrics.Sum(x=>x.Amounts.TransactionType13) },
                { "DasEarningsTransactionType14" , (double) earningsMetrics.Sum(x=>x.Amounts.TransactionType14) },
                { "DasEarningsTransactionType15" , (double) earningsMetrics.Sum(x=>x.Amounts.TransactionType15) },
                { "DasEarningsTransactionType16" , (double) earningsMetrics.Sum(x=>x.Amounts.TransactionType16) },
                { "DataLockedEarningsAmount" , (double) metrics.DataLockedEarnings},
                { "DataLockedCountDLock1" , dataLockMetrics.Sum(x=>x.Amounts.DataLock1) },
                { "DataLockedCountDLock2" , dataLockMetrics.Sum(x=>x.Amounts.DataLock2) },
                { "DataLockedCountDLock3" , dataLockMetrics.Sum(x=>x.Amounts.DataLock3) },
                { "DataLockedCountDLock4" , dataLockMetrics.Sum(x=>x.Amounts.DataLock4) },
                { "DataLockedCountDLock5" , dataLockMetrics.Sum(x=>x.Amounts.DataLock5) },
                { "DataLockedCountDLock6" , dataLockMetrics.Sum(x=>x.Amounts.DataLock6) },
                { "DataLockedCountDLock7" , dataLockMetrics.Sum(x=>x.Amounts.DataLock7) },
                { "DataLockedCountDLock8" , dataLockMetrics.Sum(x=>x.Amounts.DataLock8) },
                { "DataLockedCountDLock9" , dataLockMetrics.Sum(x=>x.Amounts.DataLock9) },
                { "DataLockedCountDLock10" , dataLockMetrics.Sum(x=>x.Amounts.DataLock10) },
                { "DataLockedCountDLock11" , dataLockMetrics.Sum(x=>x.Amounts.DataLock11) },
                { "DataLockedCountDLock12" , dataLockMetrics.Sum(x=>x.Amounts.DataLock12) },
                { "DataLockAmountAlreadyPaid" , (double) metrics.AlreadyPaidDataLockedEarnings },
                { "HeldBackCompletionPayments" ,(double) metrics.HeldBackCompletionPayments.Total },
                { "RequiredPaymentsTotal" , (double) metrics.RequiredPayments.Total },
                { "RequiredPaymentsTotalTransactionType1" , (double) requiredPaymentsMetrics.Sum(x=>x.Amounts.TransactionType1) },
                { "RequiredPaymentsTotalTransactionType2" , (double) requiredPaymentsMetrics.Sum(x=>x.Amounts.TransactionType2) },
                { "RequiredPaymentsTotalTransactionType3" , (double) requiredPaymentsMetrics.Sum(x=>x.Amounts.TransactionType3) },
                { "RequiredPaymentsTotalTransactionType4" , (double) requiredPaymentsMetrics.Sum(x=>x.Amounts.TransactionType4) },
                { "RequiredPaymentsTotalTransactionType5" , (double) requiredPaymentsMetrics.Sum(x=>x.Amounts.TransactionType5) },
                { "RequiredPaymentsTotalTransactionType6" , (double) requiredPaymentsMetrics.Sum(x=>x.Amounts.TransactionType6) },
                { "RequiredPaymentsTotalTransactionType7" , (double) requiredPaymentsMetrics.Sum(x=>x.Amounts.TransactionType7) },
                { "RequiredPaymentsTotalTransactionType8" , (double) requiredPaymentsMetrics.Sum(x=>x.Amounts.TransactionType8) },
                { "RequiredPaymentsTotalTransactionType9" , (double) requiredPaymentsMetrics.Sum(x=>x.Amounts.TransactionType9) },
                { "RequiredPaymentsTotalTransactionType10" , (double) requiredPaymentsMetrics.Sum(x=>x.Amounts.TransactionType10) },
                { "RequiredPaymentsTotalTransactionType11" , (double) requiredPaymentsMetrics.Sum(x=>x.Amounts.TransactionType11) },
                { "RequiredPaymentsTotalTransactionType12" , (double) requiredPaymentsMetrics.Sum(x=>x.Amounts.TransactionType12) },
                { "RequiredPaymentsTotalTransactionType13" , (double) requiredPaymentsMetrics.Sum(x=>x.Amounts.TransactionType13) },
                { "RequiredPaymentsTotalTransactionType14" , (double) requiredPaymentsMetrics.Sum(x=>x.Amounts.TransactionType14) },
                { "RequiredPaymentsTotalTransactionType15" , (double) requiredPaymentsMetrics.Sum(x=>x.Amounts.TransactionType15) },
                { "RequiredPaymentsTotalTransactionType16" , (double) requiredPaymentsMetrics.Sum(x=>x.Amounts.TransactionType16) },
                { "RequiredPaymentsDasEarningsDifference" , (double) submissionMetrics.DifferenceTotal }
            };

            telemetry.TrackEvent("Finished Generating Submission Metrics", properties, stats);
        }
    }
}