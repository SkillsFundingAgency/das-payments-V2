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
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

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
                SendMetricsTelemetry(overallPeriodEndSummary, stopwatch.ElapsedMilliseconds);

                logger.LogInfo($"Finished building period end metrics for {academicYear}, {collectionPeriod} using job id {jobId}, DataDuration: {dataDuration} milliseconds");
            }
            catch (Exception e)
            {
                logger.LogWarning($"Error building period end metrics for {academicYear}, {collectionPeriod} using job id {jobId}. Error: {e}");
                throw;
            }
        }

        private void SendMetricsTelemetry(PeriodEndSummaryModel metrics, long reportGenerationDuration)
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

                { "DataLockedEarningsAmount" ,                (double) metrics.AdjustedDataLockedEarnings },
                
                { "DataLockedEarningsTotal" ,                 (double) metrics.TotalDataLockedEarnings },
                
                { "DataLockAmountAlreadyPaid" ,               (double) metrics.AlreadyPaidDataLockedEarnings },
                
                
                { "HeldBackCompletionPayments" ,              (double) metrics.HeldBackCompletionPayments.Total },
                { "HeldBackCompletionPaymentsContractType1" , (double) metrics.HeldBackCompletionPayments.ContractType1 },
                { "HeldBackCompletionPaymentsContractType2" , (double) metrics.HeldBackCompletionPayments.ContractType1 },

                { "YearToDatePaymentsTotal" ,                 (double) metrics.YearToDatePayments.Total },
                { "YearToDatePaymentsContractType1Total",     (double) metrics.YearToDatePayments.ContractType1 },
                { "YearToDatePaymentsContractType2Total",     (double) metrics.YearToDatePayments.ContractType2 },
                
                
                
            };

            telemetry.TrackEvent("Finished Generating Submission Metrics", properties, stats);
        }
    }
}