using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
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
                var dataLockedAlreadyPaidTask = periodEndMetricsRepository.GetAlreadyPaidDataLockedEarnings(academicYear, collectionPeriod, cancellationToken);
                var heldBackCompletionAmountsTask = periodEndMetricsRepository.GetHeldBackCompletionPaymentsTotals(academicYear, collectionPeriod, cancellationToken);

                var dataTask = Task.WhenAll(
                    dcEarningsTask,
                    transactionTypesTask,
                    fundingSourceTask,
                    currentPaymentTotals,
                    dataLockedEarningsTask,
                    dataLockedAlreadyPaidTask,
                    heldBackCompletionAmountsTask);

                var waitTask = Task.Delay(TimeSpan.FromSeconds(270), cancellationToken);

                Task.WaitAny(dataTask, waitTask);

                cancellationToken.ThrowIfCancellationRequested();

                if (!dataTask.IsCompleted)
                    throw new InvalidOperationException($"Took too long to get data for the period end metrics. job: {jobId}, Collection period: {collectionPeriod}, Academic Year: {academicYear}");

                var providerSummaries = new List<ProviderPeriodEndSummaryModel>();

                var distinctProviderUkprns = dcEarningsTask.Result.Select(x => x.Ukprn).Distinct();
                
                var periodEndSummary = periodEndSummaryFactory.CreatePeriodEndSummary(jobId, collectionPeriod, academicYear);
                
                foreach (var ukprn in distinctProviderUkprns)
                {
                    var providerSummary = periodEndSummaryFactory.CreatePeriodEndProviderSummary(ukprn, jobId, collectionPeriod, academicYear);

                    providerSummary.AddDcEarnings(dcEarningsTask.Result.Where(x => x.Ukprn == ukprn));
                    providerSummary.AddTransactionTypes(transactionTypesTask.Result.Where(x => x.Ukprn == ukprn));
                    providerSummary.AddFundingSourceAmounts(fundingSourceTask.Result.Where(x => x.Ukprn == ukprn));
                    providerSummary.AddPaymentsYearToDate(currentPaymentTotals.Result.FirstOrDefault(x => x.Ukprn == ukprn) ?? new ProviderContractTypeAmounts());
                    providerSummary.AddDataLockedEarnings(dataLockedEarningsTask.Result.FirstOrDefault(x => x.Ukprn == ukprn)?.TotalAmount ?? 0m);
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

            var stats = new Dictionary<string, double>
            {
                { "Percentage",    (double) providerMetrics.PaymentMetrics.Percentage },
                
                { "ContractType1", (double) providerMetrics.PaymentMetrics.ContractType1 },
                { "ContractType2", (double) providerMetrics.PaymentMetrics.ContractType2 },
                
                { "DifferenceContractType1", (double) providerMetrics.PaymentMetrics.DifferenceContractType1 },
                { "DifferenceContractType2", (double) providerMetrics.PaymentMetrics.DifferenceContractType2 },
                
                { "PercentageContractType1", (double) providerMetrics.PaymentMetrics.PercentageContractType1 },
                { "PercentageContractType2", (double) providerMetrics.PaymentMetrics.PercentageContractType2 },
                
                { "EarningsDCContractType1", (double) providerMetrics.DcEarnings.ContractType1 },
                { "EarningsDCContractType2", (double) providerMetrics.DcEarnings.ContractType2 },
                
                { "PaymentsContractType1", (double) providerMetrics.Payments.ContractType1 },
                { "PaymentsContractType2", (double) providerMetrics.Payments.ContractType2 },
                
                { "DataLockedEarnings", (double) providerMetrics.AdjustedDataLockedEarnings },
                { "AlreadyPaidDataLockedEarnings", (double) providerMetrics.AlreadyPaidDataLockedEarnings },
                { "TotalDataLockedEarnings", (double) providerMetrics.TotalDataLockedEarnings },
                
                { "HeldBackCompletionPaymentsContractType1", (double) providerMetrics.HeldBackCompletionPayments.ContractType1 },
                { "HeldBackCompletionPaymentsContractType2", (double) providerMetrics.HeldBackCompletionPayments.ContractType2 },
                
                { "PaymentsYearToDateContractType1", (double) providerMetrics.YearToDatePayments.ContractType1 },
                { "PaymentsYearToDateContractType2", (double) providerMetrics.YearToDatePayments.ContractType2 },
            };

            telemetry.TrackEvent("Finished Generating Period End Metrics", properties, stats);
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
                { "ReportGenerationDuration", reportGenerationDuration },

                { "Percentage",    (double) metrics.PaymentMetrics.Percentage },
                
                { "ContractType1", (double) metrics.PaymentMetrics.ContractType1 },
                { "ContractType2", (double) metrics.PaymentMetrics.ContractType2 },
                
                { "DifferenceContractType1", (double) metrics.PaymentMetrics.DifferenceContractType1 },
                { "DifferenceContractType2", (double) metrics.PaymentMetrics.DifferenceContractType2 },
                
                { "PercentageContractType1", (double) metrics.PaymentMetrics.PercentageContractType1 },
                { "PercentageContractType2", (double) metrics.PaymentMetrics.PercentageContractType2 },
                
                { "EarningsDCContractType1", (double) metrics.DcEarnings.ContractType1 },
                { "EarningsDCContractType2", (double) metrics.DcEarnings.ContractType2 },
                
                { "PaymentsContractType1", (double) metrics.Payments.ContractType1 },
                { "PaymentsContractType2", (double) metrics.Payments.ContractType2 },
                
                { "DataLockedEarnings", (double) metrics.AdjustedDataLockedEarnings },
                { "AlreadyPaidDataLockedEarnings", (double) metrics.AlreadyPaidDataLockedEarnings },
                { "TotalDataLockedEarnings", (double) metrics.TotalDataLockedEarnings },
                
                { "HeldBackCompletionPaymentsContractType1", (double) metrics.HeldBackCompletionPayments.ContractType1 },
                { "HeldBackCompletionPaymentsContractType2", (double) metrics.HeldBackCompletionPayments.ContractType2 },
                
                { "PaymentsYearToDateContractType1", (double) metrics.YearToDatePayments.ContractType1 },
                { "PaymentsYearToDateContractType2", (double) metrics.YearToDatePayments.ContractType2 },
            };

            telemetry.TrackEvent("Finished Generating Period End Metrics", properties, stats);
        }
    }
}