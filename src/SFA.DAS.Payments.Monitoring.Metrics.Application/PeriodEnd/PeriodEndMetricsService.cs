using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Data;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.PeriodEnd
{
    public interface IPeriodEndMetricsService
    {
        Task BuildMetrics(long jobId, short academicYear, byte collectionPeriod, CancellationToken cancellationToken);
    }

    public class PeriodEndMetricsService : IPeriodEndMetricsService
    {
        private readonly IPaymentLogger logger;
        private readonly IPeriodEndSummaryFactory periodEndSummaryFactory;
        private readonly IDcMetricsDataContextFactory dcMetricsDataContextFactory;
        private readonly IPeriodEndMetricsRepository periodEndMetricsRepository;
        private readonly ITelemetry telemetry;

        public PeriodEndMetricsService(
            IPaymentLogger logger, 
            IPeriodEndSummaryFactory periodEndSummaryFactory,
            IDcMetricsDataContextFactory dcMetricsDataContextFactory, 
            IPeriodEndMetricsRepository periodEndMetricsRepository,
            ITelemetry telemetry
        )
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.periodEndSummaryFactory = periodEndSummaryFactory ?? throw new ArgumentNullException(nameof(periodEndSummaryFactory));
            this.dcMetricsDataContextFactory = dcMetricsDataContextFactory ?? throw new ArgumentNullException(nameof(dcMetricsDataContextFactory));
            this.periodEndMetricsRepository = periodEndMetricsRepository ?? throw new ArgumentNullException(nameof(periodEndMetricsRepository));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task BuildMetrics(long jobId, short academicYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogDebug($"Building period end metrics for {academicYear}, {collectionPeriod} using job id {jobId}");

                var stopwatch = Stopwatch.StartNew();
                
                var dcDataContext = dcMetricsDataContextFactory.CreateContext(academicYear);
                var dcEarningsTask = dcDataContext.GetEarnings(academicYear, collectionPeriod, cancellationToken);
                
                var transactionTypesTask = periodEndMetricsRepository.GetTransactionTypesByContractType(academicYear, collectionPeriod, cancellationToken);
                var fundingSourceTask = periodEndMetricsRepository.GetFundingSourceAmountsByContractType(academicYear, collectionPeriod, cancellationToken);
                var currentPaymentTotals = periodEndMetricsRepository.GetYearToDatePayments(academicYear, collectionPeriod, cancellationToken);
                var dataLockedEarningsTask = periodEndMetricsRepository.GetDataLockedEarningsTotals(academicYear, collectionPeriod, cancellationToken);
                var periodEndProviderDataLockTypeCountsTask = periodEndMetricsRepository.GetPeriodEndProviderDataLockTypeCounts(academicYear, collectionPeriod, cancellationToken);
                var dataLockedAlreadyPaidTask = periodEndMetricsRepository.GetAlreadyPaidDataLockedEarnings(academicYear, collectionPeriod, cancellationToken);
                var heldBackCompletionAmountsTask = periodEndMetricsRepository.GetHeldBackCompletionPaymentsTotals(academicYear, collectionPeriod, cancellationToken);

                var dataTask = Task.WhenAll(
                    dcEarningsTask,
                    transactionTypesTask,
                    fundingSourceTask,
                    currentPaymentTotals,
                    dataLockedEarningsTask,
                    periodEndProviderDataLockTypeCountsTask,
                    dataLockedAlreadyPaidTask,
                    heldBackCompletionAmountsTask);

                var waitTask = Task.Delay(TimeSpan.FromSeconds(270), cancellationToken);

                Task.WaitAny(dataTask, waitTask);

                cancellationToken.ThrowIfCancellationRequested();

                if (!dataTask.IsCompleted)
                    throw new InvalidOperationException($"Took too long to get data for the period end metrics. job: {jobId}, Collection period: {collectionPeriod}, Academic Year: {academicYear}");

                var providerSummaries = new List<ProviderPeriodEndSummaryModel>();

                var providersFromPayments = currentPaymentTotals.Result.Select(x => x.Ukprn).Distinct();
                var providersFromEarnings = dcEarningsTask.Result.Select(x => x.Ukprn).Distinct();
                var distinctProviderUkprns = providersFromEarnings.Union(providersFromPayments);
                
                var periodEndSummary = periodEndSummaryFactory.CreatePeriodEndSummary(jobId, collectionPeriod, academicYear);
                
                foreach (var ukprn in distinctProviderUkprns)
                {
                    var providerSummary = periodEndSummaryFactory.CreatePeriodEndProviderSummary(ukprn, jobId, collectionPeriod, academicYear);

                    providerSummary.AddDcEarnings(dcEarningsTask.Result.Where(x => x.Ukprn == ukprn));
                    providerSummary.AddTransactionTypes(transactionTypesTask.Result.Where(x => x.Ukprn == ukprn));
                    providerSummary.AddFundingSourceAmounts(fundingSourceTask.Result.Where(x => x.Ukprn == ukprn));
                    providerSummary.AddPaymentsYearToDate(currentPaymentTotals.Result.FirstOrDefault(x => x.Ukprn == ukprn) ?? new ProviderContractTypeAmounts());
                    providerSummary.AddDataLockedEarnings(dataLockedEarningsTask.Result.FirstOrDefault(x => x.Ukprn == ukprn)?.TotalAmount ?? 0m);
                    providerSummary.AddPeriodEndProviderDataLockTypeCounts(periodEndProviderDataLockTypeCountsTask.Result.FirstOrDefault(x => x.Ukprn == ukprn) ?? new PeriodEndProviderDataLockTypeCounts());
                    providerSummary.AddDataLockedAlreadyPaid(dataLockedAlreadyPaidTask.Result.FirstOrDefault(x => x.Ukprn == ukprn)?.TotalAmount ?? 0m);
                    providerSummary.AddHeldBackCompletionPayments(heldBackCompletionAmountsTask.Result.FirstOrDefault(x => x.Ukprn == ukprn) ?? new ProviderContractTypeAmounts());

                    var providerSummaryModel = providerSummary.GetMetrics();

                    providerSummaries.Add(providerSummaryModel);
                }

                periodEndSummary.AddProviderSummaries(providerSummaries);

                var overallPeriodEndSummary = periodEndSummary.GetMetrics();

                stopwatch.Stop();

                var dataDuration = stopwatch.ElapsedMilliseconds;
                
                await periodEndMetricsRepository.SaveProviderSummaries(providerSummaries, overallPeriodEndSummary, cancellationToken);
                SendSummaryMetricsTelemetry(overallPeriodEndSummary, stopwatch.ElapsedMilliseconds);
                SendAllProviderMetricsTelemetry(providerSummaries, overallPeriodEndSummary);

                logger.LogInfo($"Finished building period end metrics for {academicYear}, {collectionPeriod} using job id {jobId}, DataDuration: {dataDuration} milliseconds");
            }
            catch (Exception e)
            {
                logger.LogWarning($"Error building period end metrics for {academicYear}, {collectionPeriod} using job id {jobId}. Error: {e}");
                throw;
            }
        }

        private void SendProviderMetricsTelemetry(ProviderPeriodEndSummaryModel providerMetrics, PeriodEndSummaryModel overallMetrics)
        {
            var properties = new Dictionary<string, string>
            {
                { TelemetryKeys.JobId, providerMetrics.JobId.ToString()},
                { TelemetryKeys.CollectionPeriod, overallMetrics.CollectionPeriod.ToString()},
                { TelemetryKeys.AcademicYear, overallMetrics.AcademicYear.ToString()},
                { TelemetryKeys.Ukprn, providerMetrics.Ukprn.ToString() },
            };

            var act1FundingSource = providerMetrics
                .FundingSourceAmounts
                .Where(x => x.ContractType == ContractType.Act1)
                .ToList();
            var act2FundingSource = providerMetrics
                .FundingSourceAmounts
                .Where(x => x.ContractType == ContractType.Act2)
                .ToList();

            var act1TransactionTypes = providerMetrics
                .TransactionTypeAmounts
                .Where(x => x.TransactionTypeAmounts.ContractType == ContractType.Act1)
                .ToList();
            var act2TransactionTypes = providerMetrics
                .TransactionTypeAmounts
                .Where(x => x.TransactionTypeAmounts.ContractType == ContractType.Act2)
                .ToList();

            var stats = new Dictionary<string, double>
            {
                { "Percentage",    (double) providerMetrics.PaymentMetrics.Percentage },
                { "PercentageContractType1", (double) providerMetrics.PaymentMetrics.PercentageContractType1 },
                { "PercentageContractType2", (double) providerMetrics.PaymentMetrics.PercentageContractType2 },

                { "Total", (double) providerMetrics.PaymentMetrics.Total },
                { "ContractType1", (double) providerMetrics.PaymentMetrics.ContractType1 },
                { "ContractType2", (double) providerMetrics.PaymentMetrics.ContractType2 },
                
                { "DifferenceTotal", (double) providerMetrics.PaymentMetrics.DifferenceTotal },
                { "DifferenceContractType1", (double) providerMetrics.PaymentMetrics.DifferenceContractType1 },
                { "DifferenceContractType2", (double) providerMetrics.PaymentMetrics.DifferenceContractType2 },
                
                
                { "EarningsDCTotal", (double) providerMetrics.DcEarnings.Total },
                { "EarningsDCContractType1", (double) providerMetrics.DcEarnings.ContractType1 },
                { "EarningsDCContractType2", (double) providerMetrics.DcEarnings.ContractType2 },
                
                { "PaymentsTotal", (double) providerMetrics.Payments.Total },
                { "PaymentsContractType1", (double) providerMetrics.Payments.ContractType1 },
                { "PaymentsContractType2", (double) providerMetrics.Payments.ContractType2 },
                
                { "DataLockedEarnings", (double) providerMetrics.AdjustedDataLockedEarnings },
                { "AlreadyPaidDataLockedEarnings", (double) providerMetrics.AlreadyPaidDataLockedEarnings },
                { "TotalDataLockedEarnings", (double) providerMetrics.TotalDataLockedEarnings },

                { "DataLockedCountDLock1" ,  (double) providerMetrics.DataLockTypeCounts.DataLock1 },
                { "DataLockedCountDLock2" ,  (double) providerMetrics.DataLockTypeCounts.DataLock2 },
                { "DataLockedCountDLock3" ,  (double) providerMetrics.DataLockTypeCounts.DataLock3 },
                { "DataLockedCountDLock4" ,  (double) providerMetrics.DataLockTypeCounts.DataLock4 },
                { "DataLockedCountDLock5" ,  (double) providerMetrics.DataLockTypeCounts.DataLock5 },
                { "DataLockedCountDLock6" ,  (double) providerMetrics.DataLockTypeCounts.DataLock6 },
                { "DataLockedCountDLock7" ,  (double) providerMetrics.DataLockTypeCounts.DataLock7 },
                { "DataLockedCountDLock8" ,  (double) providerMetrics.DataLockTypeCounts.DataLock8 },
                { "DataLockedCountDLock9" ,  (double) providerMetrics.DataLockTypeCounts.DataLock9 },
                { "DataLockedCountDLock10" , (double) providerMetrics.DataLockTypeCounts.DataLock10 },
                { "DataLockedCountDLock11" , (double) providerMetrics.DataLockTypeCounts.DataLock11 },
                { "DataLockedCountDLock12" , (double) providerMetrics.DataLockTypeCounts.DataLock12 },

                { "HeldBackCompletionPaymentsTotal", (double) providerMetrics.HeldBackCompletionPayments.Total },
                { "HeldBackCompletionPaymentsContractType1", (double) providerMetrics.HeldBackCompletionPayments.ContractType1 },
                { "HeldBackCompletionPaymentsContractType2", (double) providerMetrics.HeldBackCompletionPayments.ContractType2 },

                { "PaymentsYearToDateTotal", (double) providerMetrics.YearToDatePayments.Total },
                { "PaymentsYearToDateContractType1", (double) providerMetrics.YearToDatePayments.ContractType1 },
                { "PaymentsYearToDateContractType2", (double) providerMetrics.YearToDatePayments.ContractType2 },


                { "ContractType1FundingSourceTotal", (double) act1FundingSource.Sum(x => x.Total) },
                { "ContractType1FundingSource1", (double) act1FundingSource.Sum(x => x.FundingSource1) },
                { "ContractType1FundingSource2", (double) act1FundingSource.Sum(x => x.FundingSource2) },
                { "ContractType1FundingSource3", (double) act1FundingSource.Sum(x => x.FundingSource3) },
                { "ContractType1FundingSource4", (double) act1FundingSource.Sum(x => x.FundingSource4) },
                { "ContractType1FundingSource5", (double) act1FundingSource.Sum(x => x.FundingSource5) },

                { "ContractType2FundingSourceTotal", (double) act2FundingSource.Sum(x => x.Total) },
                { "ContractType2FundingSource1", (double) act2FundingSource.Sum(x => x.FundingSource1) },
                { "ContractType2FundingSource2", (double) act2FundingSource.Sum(x => x.FundingSource2) },
                { "ContractType2FundingSource3", (double) act2FundingSource.Sum(x => x.FundingSource3) },
                { "ContractType2FundingSource4", (double) act2FundingSource.Sum(x => x.FundingSource4) },
                { "ContractType2FundingSource5", (double) act2FundingSource.Sum(x => x.FundingSource5) },


                {"ContractType1TransactionTypeTotal", (double) act1TransactionTypes.Sum(x => x.TransactionTypeAmounts.Total)},
                {"ContractType1TransactionType01", (double) act1TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType1)},
                {"ContractType1TransactionType02", (double) act1TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType2)},
                {"ContractType1TransactionType03", (double) act1TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType3)},
                {"ContractType1TransactionType04", (double) act1TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType4)},
                {"ContractType1TransactionType05", (double) act1TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType5)},
                {"ContractType1TransactionType06", (double) act1TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType6)},
                {"ContractType1TransactionType07", (double) act1TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType7)},
                {"ContractType1TransactionType08", (double) act1TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType8)},
                {"ContractType1TransactionType09", (double) act1TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType9)},
                {"ContractType1TransactionType10", (double) act1TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType10)},
                {"ContractType1TransactionType11", (double) act1TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType11)},
                {"ContractType1TransactionType12", (double) act1TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType12)},
                {"ContractType1TransactionType13", (double) act1TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType13)},
                {"ContractType1TransactionType14", (double) act1TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType14)},
                {"ContractType1TransactionType15", (double) act1TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType15)},
                {"ContractType1TransactionType16", (double) act1TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType16)},

                {"ContractType2TransactionTypeTotal", (double) act2TransactionTypes.Sum(x => x.TransactionTypeAmounts.Total)},
                {"ContractType2TransactionType01", (double) act2TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType1)},
                {"ContractType2TransactionType02", (double) act2TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType2)},
                {"ContractType2TransactionType03", (double) act2TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType3)},
                {"ContractType2TransactionType04", (double) act2TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType4)},
                {"ContractType2TransactionType05", (double) act2TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType5)},
                {"ContractType2TransactionType06", (double) act2TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType6)},
                {"ContractType2TransactionType07", (double) act2TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType7)},
                {"ContractType2TransactionType08", (double) act2TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType8)},
                {"ContractType2TransactionType09", (double) act2TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType9)},
                {"ContractType2TransactionType10", (double) act2TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType10)},
                {"ContractType2TransactionType11", (double) act2TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType11)},
                {"ContractType2TransactionType12", (double) act2TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType12)},
                {"ContractType2TransactionType13", (double) act2TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType13)},
                {"ContractType2TransactionType14", (double) act2TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType14)},
                {"ContractType2TransactionType15", (double) act2TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType15)},
                {"ContractType2TransactionType16", (double) act2TransactionTypes.Sum(x => x.TransactionTypeAmounts.TransactionType16)},

            };

            telemetry.TrackEvent($"Finished Generating Period End Metrics for Provider: {providerMetrics.Ukprn}", properties, stats);
        }

        private void SendAllProviderMetricsTelemetry(List<ProviderPeriodEndSummaryModel> providerMetrics, PeriodEndSummaryModel metrics)
        {
            foreach (var providerMetric in providerMetrics)
            {
                SendProviderMetricsTelemetry(providerMetric, metrics);
            }
        }

        private void SendSummaryMetricsTelemetry(PeriodEndSummaryModel metrics, long reportGenerationDuration)
        {
            var properties = new Dictionary<string, string>
            {
                { TelemetryKeys.JobId, metrics.JobId.ToString()},
                { TelemetryKeys.CollectionPeriod, metrics.CollectionPeriod.ToString()},
                { TelemetryKeys.AcademicYear, metrics.AcademicYear.ToString()},
            };

            var stats = new Dictionary<string, double>
            {
                { "Percentage",    (double) metrics.Percentage },
                
                { "ContractType1", (double) metrics.PaymentMetrics.ContractType1 },
                { "ContractType2", (double) metrics.PaymentMetrics.ContractType2 },
                
                { "DifferenceTotal", (double) metrics.PaymentMetrics.DifferenceTotal },
                { "DifferenceContractType1", (double) metrics.PaymentMetrics.DifferenceContractType1 },
                { "DifferenceContractType2", (double) metrics.PaymentMetrics.DifferenceContractType2 },
                
                { "PercentageTotal", (double) metrics.PaymentMetrics.Percentage },
                { "PercentageContractType1", (double) metrics.PaymentMetrics.PercentageContractType1 },
                { "PercentageContractType2", (double) metrics.PaymentMetrics.PercentageContractType2 },
                
                { "EarningsDCTotal", (double) metrics.DcEarnings.Total },
                { "EarningsDCContractType1", (double) metrics.DcEarnings.ContractType1 },
                { "EarningsDCContractType2", (double) metrics.DcEarnings.ContractType2 },
                
                { "PaymentsTotal", (double) metrics.Payments.Total },
                { "PaymentsContractType1", (double) metrics.Payments.ContractType1 },
                { "PaymentsContractType2", (double) metrics.Payments.ContractType2 },
                
                { "DataLockedEarnings", (double) metrics.AdjustedDataLockedEarnings },
                { "AlreadyPaidDataLockedEarnings", (double) metrics.AlreadyPaidDataLockedEarnings },
                { "TotalDataLockedEarnings", (double) metrics.TotalDataLockedEarnings },

                { "DataLockedCountDLock1" ,  (double) metrics.DataLockTypeCounts.DataLock1 },
                { "DataLockedCountDLock2" ,  (double) metrics.DataLockTypeCounts.DataLock2 },
                { "DataLockedCountDLock3" ,  (double) metrics.DataLockTypeCounts.DataLock3 },
                { "DataLockedCountDLock4" ,  (double) metrics.DataLockTypeCounts.DataLock4 },
                { "DataLockedCountDLock5" ,  (double) metrics.DataLockTypeCounts.DataLock5 },
                { "DataLockedCountDLock6" ,  (double) metrics.DataLockTypeCounts.DataLock6 },
                { "DataLockedCountDLock7" ,  (double) metrics.DataLockTypeCounts.DataLock7 },
                { "DataLockedCountDLock8" ,  (double) metrics.DataLockTypeCounts.DataLock8 },
                { "DataLockedCountDLock9" ,  (double) metrics.DataLockTypeCounts.DataLock9 },
                { "DataLockedCountDLock10" , (double) metrics.DataLockTypeCounts.DataLock10 },
                { "DataLockedCountDLock11" , (double) metrics.DataLockTypeCounts.DataLock11 },
                { "DataLockedCountDLock12" , (double) metrics.DataLockTypeCounts.DataLock12 },

                { "HeldBackCompletionPaymentsTotal", (double) metrics.HeldBackCompletionPayments.Total },
                { "HeldBackCompletionPaymentsContractType1", (double) metrics.HeldBackCompletionPayments.ContractType1 },
                { "HeldBackCompletionPaymentsContractType2", (double) metrics.HeldBackCompletionPayments.ContractType2 },
                
                { "PaymentsYearToDateTotal", (double) metrics.YearToDatePayments.Total },
                { "PaymentsYearToDateContractType1", (double) metrics.YearToDatePayments.ContractType1 },
                { "PaymentsYearToDateContractType2", (double) metrics.YearToDatePayments.ContractType2 },

            };

            telemetry.TrackEvent("Finished Generating Period End Metrics", properties, stats);
        }
    }
}