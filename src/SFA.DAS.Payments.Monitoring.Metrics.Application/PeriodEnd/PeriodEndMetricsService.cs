using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
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

        public PeriodEndMetricsService(
            IPaymentLogger logger, 
            IPeriodEndSummaryFactory periodEndSummaryFactory,
            IDcMetricsDataContextFactory dcMetricsDataContextFactory, 
            IPeriodEndMetricsRepository periodEndMetricsRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.periodEndSummaryFactory = periodEndSummaryFactory ?? throw new ArgumentNullException(nameof(periodEndSummaryFactory));
            this.dcMetricsDataContextFactory = dcMetricsDataContextFactory ?? throw new ArgumentNullException(nameof(dcMetricsDataContextFactory));
            this.periodEndMetricsRepository = periodEndMetricsRepository ?? throw new ArgumentNullException(nameof(periodEndMetricsRepository));
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
                    dataLockedEarningsTask,
                    dataLockedAlreadyPaidTask);

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

                logger.LogInfo($"Finished building period end metrics for {academicYear}, {collectionPeriod} using job id {jobId}, DataDuration: {dataDuration} milliseconds");
            }
            catch (Exception e)
            {
                logger.LogWarning($"Error building period end metrics for {academicYear}, {collectionPeriod} using job id {jobId}. Error: {e}");
                throw;
            }
        }
    }
}