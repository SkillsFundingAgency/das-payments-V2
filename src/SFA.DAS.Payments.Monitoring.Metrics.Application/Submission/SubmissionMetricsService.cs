using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Model.Core.Entities;
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
                var dcEarningsTask = dcMetricsDataContextFactory.CreateContext(academicYear).GetEarnings(ukprn, academicYear, collectionPeriod, cancellationToken);
                
                var dasEarningsTask = submissionRepository.GetDasEarnings(ukprn, jobId, cancellationToken);
                var dataLocksTask = submissionRepository.GetDataLockedEarnings(ukprn, jobId, cancellationToken);
                var dataLocksTotalTask = submissionRepository.GetDataLockedEarningsTotal(ukprn, jobId, cancellationToken);
                var dataLocksAlreadyPaid = submissionRepository.GetAlreadyPaidDataLockedEarnings(ukprn, jobId, cancellationToken);
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
            var dasEarnings = metrics.DasEarnings;
            var dcEarnings = metrics.DcEarnings;
            // ReSharper disable InconsistentNaming this is to make variable name easy to read 
            var das_earningsMetrics = metrics.EarningsMetrics.Where(x=>x.EarningsType == EarningsType.Das).ToList();
            var dc_earningsMetrics = metrics.EarningsMetrics.Where(x=>x.EarningsType == EarningsType.Dc).ToList();
            var dataLockMetrics = metrics.DataLockMetrics;
            var requiredPayments_Act1_Metrics = metrics.RequiredPaymentsMetrics.Where(x => x.Amounts.ContractType == ContractType.Act1).ToList();
            var requiredPayments_Act2_Metrics = metrics.RequiredPaymentsMetrics.Where(x => x.Amounts.ContractType == ContractType.Act2).ToList();

            var stats = new Dictionary<string, double>
            {
                { "ReportGenerationDuration", reportGenerationDuration },

                { "Percentage" ,                         (double) submissionMetrics.Percentage },
                { "ContractType1Percentage" ,            (double) submissionMetrics.PercentageContractType1 },
                { "ContractType2Percentage" ,            (double) submissionMetrics.PercentageContractType2 },
                
                { "DifferenceTotal" ,                    (double) submissionMetrics.DifferenceTotal },
                { "DifferenceContractType1" ,            (double) submissionMetrics.DifferenceContractType1 },
                { "DifferenceContractType2" ,            (double) submissionMetrics.DifferenceContractType2 },
                
                { "EarningsPercentage" ,                 (double) dasEarnings.Percentage },
                { "EarningsPercentageContractType1" ,    (double) dasEarnings.PercentageContractType1 },
                { "EarningsPercentageContractType2" ,    (double) dasEarnings.PercentageContractType2 },
   
                { "EarningsDifferenceTotal" ,            (double) dasEarnings.DifferenceTotal },
                { "EarningsDifferenceContractType1" ,    (double) dasEarnings.DifferenceContractType1 },
                { "EarningsDifferenceContractType2" ,    (double) dasEarnings.DifferenceContractType2 },

                { "DasEarningsTotal" ,                   (double) dasEarnings.Total },
                { "DasEarningsContractType1Total" ,      (double) dasEarnings.ContractType1 },
                { "DasEarningsContractType2Total" ,      (double) dasEarnings.ContractType2 },
                
                { "DcEarningsTotal" ,                    (double) dcEarnings.Total },
                { "DcEarningsContractType1Total" ,       (double) dcEarnings.ContractType1 },
                { "DcEarningsContractType2Total" ,       (double) dcEarnings.ContractType2 },
                
                { "DataLockedEarningsAmount" ,                (double) metrics.AdjustedDataLockedEarnings },
                
                { "DataLockedEarningsTotal" ,                 (double) metrics.TotalDataLockedEarnings },
                
                { "DataLockAmountAlreadyPaid" ,               (double) metrics.AlreadyPaidDataLockedEarnings },
                
                { "HeldBackCompletionPayments" ,              (double) metrics.HeldBackCompletionPayments.Total },
                { "HeldBackCompletionPaymentsContractType1" , (double) metrics.HeldBackCompletionPayments.ContractType1 },
                { "HeldBackCompletionPaymentsContractType2" , (double) metrics.HeldBackCompletionPayments.ContractType1 },

                { "YearToDatePaymentsTotal" ,                 (double) metrics.YearToDatePayments.Total },
                { "YearToDatePaymentsContractType1Total",     (double) metrics.YearToDatePayments.ContractType1 },
                { "YearToDatePaymentsContractType2Total",     (double) metrics.YearToDatePayments.ContractType2 },
                
                { "DasEarningsTransactionType1" ,  (double) das_earningsMetrics.Sum(x=>x.Amounts.TransactionType1) },
                { "DasEarningsTransactionType2" ,  (double) das_earningsMetrics.Sum(x=>x.Amounts.TransactionType2) },
                { "DasEarningsTransactionType3" ,  (double) das_earningsMetrics.Sum(x=>x.Amounts.TransactionType3) },
                { "DasEarningsTransactionType4" ,  (double) das_earningsMetrics.Sum(x=>x.Amounts.TransactionType4) },
                { "DasEarningsTransactionType5" ,  (double) das_earningsMetrics.Sum(x=>x.Amounts.TransactionType5) },
                { "DasEarningsTransactionType6" ,  (double) das_earningsMetrics.Sum(x=>x.Amounts.TransactionType6) },
                { "DasEarningsTransactionType7" ,  (double) das_earningsMetrics.Sum(x=>x.Amounts.TransactionType7) },
                { "DasEarningsTransactionType8" ,  (double) das_earningsMetrics.Sum(x=>x.Amounts.TransactionType8) },
                { "DasEarningsTransactionType9" ,  (double) das_earningsMetrics.Sum(x=>x.Amounts.TransactionType9) },
                { "DasEarningsTransactionType10" , (double) das_earningsMetrics.Sum(x=>x.Amounts.TransactionType10) },
                { "DasEarningsTransactionType11" , (double) das_earningsMetrics.Sum(x=>x.Amounts.TransactionType11) },
                { "DasEarningsTransactionType12" , (double) das_earningsMetrics.Sum(x=>x.Amounts.TransactionType12) },
                { "DasEarningsTransactionType13" , (double) das_earningsMetrics.Sum(x=>x.Amounts.TransactionType13) },
                { "DasEarningsTransactionType14" , (double) das_earningsMetrics.Sum(x=>x.Amounts.TransactionType14) },
                { "DasEarningsTransactionType15" , (double) das_earningsMetrics.Sum(x=>x.Amounts.TransactionType15) },
                { "DasEarningsTransactionType16" , (double) das_earningsMetrics.Sum(x=>x.Amounts.TransactionType16) },
                
                { "DcEarningsTransactionType1" ,  (double) dc_earningsMetrics.Sum(x=>x.Amounts.TransactionType1) },
                { "DcEarningsTransactionType2" ,  (double) dc_earningsMetrics.Sum(x=>x.Amounts.TransactionType2) },
                { "DcEarningsTransactionType3" ,  (double) dc_earningsMetrics.Sum(x=>x.Amounts.TransactionType3) },
                { "DcEarningsTransactionType4" ,  (double) dc_earningsMetrics.Sum(x=>x.Amounts.TransactionType4) },
                { "DcEarningsTransactionType5" ,  (double) dc_earningsMetrics.Sum(x=>x.Amounts.TransactionType5) },
                { "DcEarningsTransactionType6" ,  (double) dc_earningsMetrics.Sum(x=>x.Amounts.TransactionType6) },
                { "DcEarningsTransactionType7" ,  (double) dc_earningsMetrics.Sum(x=>x.Amounts.TransactionType7) },
                { "DcEarningsTransactionType8" ,  (double) dc_earningsMetrics.Sum(x=>x.Amounts.TransactionType8) },
                { "DcEarningsTransactionType9" ,  (double) dc_earningsMetrics.Sum(x=>x.Amounts.TransactionType9) },
                { "DcEarningsTransactionType10" , (double) dc_earningsMetrics.Sum(x=>x.Amounts.TransactionType10) },
                { "DcEarningsTransactionType11" , (double) dc_earningsMetrics.Sum(x=>x.Amounts.TransactionType11) },
                { "DcEarningsTransactionType12" , (double) dc_earningsMetrics.Sum(x=>x.Amounts.TransactionType12) },
                { "DcEarningsTransactionType13" , (double) dc_earningsMetrics.Sum(x=>x.Amounts.TransactionType13) },
                { "DcEarningsTransactionType14" , (double) dc_earningsMetrics.Sum(x=>x.Amounts.TransactionType14) },
                { "DcEarningsTransactionType15" , (double) dc_earningsMetrics.Sum(x=>x.Amounts.TransactionType15) },
                { "DcEarningsTransactionType16" , (double) dc_earningsMetrics.Sum(x=>x.Amounts.TransactionType16) },
                
                { "DataLockedCountDLock1" ,  dataLockMetrics.Sum(x=>x.Amounts.DataLock1) },
                { "DataLockedCountDLock2" ,  dataLockMetrics.Sum(x=>x.Amounts.DataLock2) },
                { "DataLockedCountDLock3" ,  dataLockMetrics.Sum(x=>x.Amounts.DataLock3) },
                { "DataLockedCountDLock4" ,  dataLockMetrics.Sum(x=>x.Amounts.DataLock4) },
                { "DataLockedCountDLock5" ,  dataLockMetrics.Sum(x=>x.Amounts.DataLock5) },
                { "DataLockedCountDLock6" ,  dataLockMetrics.Sum(x=>x.Amounts.DataLock6) },
                { "DataLockedCountDLock7" ,  dataLockMetrics.Sum(x=>x.Amounts.DataLock7) },
                { "DataLockedCountDLock8" ,  dataLockMetrics.Sum(x=>x.Amounts.DataLock8) },
                { "DataLockedCountDLock9" ,  dataLockMetrics.Sum(x=>x.Amounts.DataLock9) },
                { "DataLockedCountDLock10" , dataLockMetrics.Sum(x=>x.Amounts.DataLock10) },
                { "DataLockedCountDLock11" , dataLockMetrics.Sum(x=>x.Amounts.DataLock11) },
                { "DataLockedCountDLock12" , dataLockMetrics.Sum(x=>x.Amounts.DataLock12) },
                
                { "RequiredPaymentsTotal" ,     (double) metrics.RequiredPayments.Total },
                { "RequiredPaymentsAct1Total" , (double) metrics.RequiredPayments.ContractType1 },
                { "RequiredPaymentsAc2Total" ,  (double) metrics.RequiredPayments.ContractType2 },
                
                { "RequiredPaymentsAct1TotalTransactionType1" ,  (double) requiredPayments_Act1_Metrics.Sum(x=>x.Amounts.TransactionType1) },
                { "RequiredPaymentsAct1TotalTransactionType2" ,  (double) requiredPayments_Act1_Metrics.Sum(x=>x.Amounts.TransactionType2) },
                { "RequiredPaymentsAct1TotalTransactionType3" ,  (double) requiredPayments_Act1_Metrics.Sum(x=>x.Amounts.TransactionType3) },
                { "RequiredPaymentsAct1TotalTransactionType4" ,  (double) requiredPayments_Act1_Metrics.Sum(x=>x.Amounts.TransactionType4) },
                { "RequiredPaymentsAct1TotalTransactionType5" ,  (double) requiredPayments_Act1_Metrics.Sum(x=>x.Amounts.TransactionType5) },
                { "RequiredPaymentsAct1TotalTransactionType6" ,  (double) requiredPayments_Act1_Metrics.Sum(x=>x.Amounts.TransactionType6) },
                { "RequiredPaymentsAct1TotalTransactionType7" ,  (double) requiredPayments_Act1_Metrics.Sum(x=>x.Amounts.TransactionType7) },
                { "RequiredPaymentsAct1TotalTransactionType8" ,  (double) requiredPayments_Act1_Metrics.Sum(x=>x.Amounts.TransactionType8) },
                { "RequiredPaymentsAct1TotalTransactionType9" ,  (double) requiredPayments_Act1_Metrics.Sum(x=>x.Amounts.TransactionType9) },
                { "RequiredPaymentsAct1TotalTransactionType10" , (double) requiredPayments_Act1_Metrics.Sum(x=>x.Amounts.TransactionType10) },
                { "RequiredPaymentsAct1TotalTransactionType11" , (double) requiredPayments_Act1_Metrics.Sum(x=>x.Amounts.TransactionType11) },
                { "RequiredPaymentsAct1TotalTransactionType12" , (double) requiredPayments_Act1_Metrics.Sum(x=>x.Amounts.TransactionType12) },
                { "RequiredPaymentsAct1TotalTransactionType13" , (double) requiredPayments_Act1_Metrics.Sum(x=>x.Amounts.TransactionType13) },
                { "RequiredPaymentsAct1TotalTransactionType14" , (double) requiredPayments_Act1_Metrics.Sum(x=>x.Amounts.TransactionType14) },
                { "RequiredPaymentsAct1TotalTransactionType15" , (double) requiredPayments_Act1_Metrics.Sum(x=>x.Amounts.TransactionType15) },
                { "RequiredPaymentsAct1TotalTransactionType16" , (double) requiredPayments_Act1_Metrics.Sum(x=>x.Amounts.TransactionType16) },
                
                { "RequiredPaymentsAct2TotalTransactionType1" ,  (double) requiredPayments_Act2_Metrics.Sum(x=>x.Amounts.TransactionType1) },
                { "RequiredPaymentsAct2TotalTransactionType2" ,  (double) requiredPayments_Act2_Metrics.Sum(x=>x.Amounts.TransactionType2) },
                { "RequiredPaymentsAct2TotalTransactionType3" ,  (double) requiredPayments_Act2_Metrics.Sum(x=>x.Amounts.TransactionType3) },
                { "RequiredPaymentsAct2TotalTransactionType4" ,  (double) requiredPayments_Act2_Metrics.Sum(x=>x.Amounts.TransactionType4) },
                { "RequiredPaymentsAct2TotalTransactionType5" ,  (double) requiredPayments_Act2_Metrics.Sum(x=>x.Amounts.TransactionType5) },
                { "RequiredPaymentsAct2TotalTransactionType6" ,  (double) requiredPayments_Act2_Metrics.Sum(x=>x.Amounts.TransactionType6) },
                { "RequiredPaymentsAct2TotalTransactionType7" ,  (double) requiredPayments_Act2_Metrics.Sum(x=>x.Amounts.TransactionType7) },
                { "RequiredPaymentsAct2TotalTransactionType8" ,  (double) requiredPayments_Act2_Metrics.Sum(x=>x.Amounts.TransactionType8) },
                { "RequiredPaymentsAct2TotalTransactionType9" ,  (double) requiredPayments_Act2_Metrics.Sum(x=>x.Amounts.TransactionType9) },
                { "RequiredPaymentsAct2TotalTransactionType10" , (double) requiredPayments_Act2_Metrics.Sum(x=>x.Amounts.TransactionType10) },
                { "RequiredPaymentsAct2TotalTransactionType11" , (double) requiredPayments_Act2_Metrics.Sum(x=>x.Amounts.TransactionType11) },
                { "RequiredPaymentsAct2TotalTransactionType12" , (double) requiredPayments_Act2_Metrics.Sum(x=>x.Amounts.TransactionType12) },
                { "RequiredPaymentsAct2TotalTransactionType13" , (double) requiredPayments_Act2_Metrics.Sum(x=>x.Amounts.TransactionType13) },
                { "RequiredPaymentsAct2TotalTransactionType14" , (double) requiredPayments_Act2_Metrics.Sum(x=>x.Amounts.TransactionType14) },
                { "RequiredPaymentsAct2TotalTransactionType15" , (double) requiredPayments_Act2_Metrics.Sum(x=>x.Amounts.TransactionType15) },
                { "RequiredPaymentsAct2TotalTransactionType16" , (double) requiredPayments_Act2_Metrics.Sum(x=>x.Amounts.TransactionType16) },
                
                { "RequiredPaymentsDasEarningsPercentageComparison" ,  Math.Round(((double) (metrics.YearToDatePayments.Total + metrics.RequiredPayments.Total) / (double) metrics.DasEarnings.Total) * 100, 2) }
            };

            telemetry.TrackEvent("Finished Generating Submission Metrics", properties, stats);
        }
    }
}