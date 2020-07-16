using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Metrics.Data;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

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
        private readonly IPeriodEndMetricsRepository periodEndMetricsRepository;

        public PeriodEndMetricsService(IPaymentLogger logger, IPeriodEndSummaryFactory periodEndSummaryFactory,
            IDcMetricsDataContext dcDataContext, IPeriodEndMetricsRepository periodEndMetricsRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.periodEndSummaryFactory = periodEndSummaryFactory ??
                                           throw new ArgumentNullException(nameof(periodEndSummaryFactory));
            this.dcDataContext = dcDataContext ?? throw new ArgumentNullException(nameof(dcDataContext));
            this.periodEndMetricsRepository = periodEndMetricsRepository ??
                                              throw new ArgumentNullException(nameof(periodEndMetricsRepository));
        }

        public async Task BuildMetrics(long jobId, short academicYear, byte collectionPeriod,
            CancellationToken cancellationToken)
        {
            try
            {
                logger.LogDebug(
                    $"Building period end metrics for {academicYear}, {collectionPeriod} using job id {jobId}");

                var stopwatch = Stopwatch.StartNew();

                var dcEarningsTask = dcDataContext.GetEarnings(academicYear, collectionPeriod, cancellationToken);
                var transactionTypesTask =
                    periodEndMetricsRepository.GetTransactionTypesByContractType(academicYear, collectionPeriod,
                        cancellationToken);
                var fundingSourceTask =
                    periodEndMetricsRepository.GetFundingSourceAmountsByContractType(academicYear, collectionPeriod,
                        cancellationToken);
                //get payments totals by provider per contract type
                //might not need this???
                //get data-locked amounts by provider
                var dataLockedEarningsTask =
                    periodEndMetricsRepository.GetDataLockedEarningsTotals(academicYear, collectionPeriod, cancellationToken);
                //var dataLockedAlreadyPaidTask =
                //    periodEndMetricsRepository.GetAlreadyPaidDataLockedEarnings(academicYear, collectionPeriod,
                //        cancellationToken);
                //var heldBackCompletionAmountsTask = periodEndMetricsRepository.GetHeldBackCompletionPaymentsTotals(academicYear, collectionPeriod, cancellationToken);
                ////get held back completion payments by provider

                var dataTask = Task.WhenAll(dcEarningsTask, transactionTypesTask, fundingSourceTask, dataLockedEarningsTask);
                var waitTask = Task.Delay(TimeSpan.FromSeconds(270), cancellationToken);
                Task.WaitAny(dataTask, waitTask);
                cancellationToken.ThrowIfCancellationRequested();
                if (!dataTask.IsCompleted)
                    throw new InvalidOperationException(
                        $"Took too long to get data for the period end metrics. job: {jobId}, Collection period: {collectionPeriod}, Academic Year: {academicYear}");


                var providerSummaries = new List<ProviderPeriodEndSummaryModel>();

                var allUkprn = dcEarningsTask.Result.Select(x => x.Ukprn).Distinct();
                var periodEndSummary =
                    periodEndSummaryFactory.CreatePeriodEndSummary(jobId, collectionPeriod, academicYear);
                foreach (var ukprn in allUkprn)
                {
                    var providerSummary =
                        periodEndSummaryFactory.CreatePeriodEndProviderSummary(ukprn, jobId, collectionPeriod,
                            academicYear);

                    //call method to add:
                    providerSummary.AddDcEarning(dcEarningsTask.Result.Where(x => x.Ukprn == ukprn));
                    ////payment by provider /transactiontype/contractype
                    providerSummary.AddTransactionTypes(
                        transactionTypesTask.Result.Where(x => x.Ukprn == ukprn));
                    ////add payments by provider by funding source per contract type
                    providerSummary.AddFundingSourceAmounts(fundingSourceTask.Result.Where(x => x.Ukprn == ukprn));
                    //add payments totals by provider per contract type
                    //might not need this???
                    //add data-locked earnings ??
                    providerSummary.AddDataLockedEarnings(dataLockedEarningsTask.Result.FirstOrDefault(x => x.Ukprn == ukprn)?.TotalAmount ?? 0m);

                    //add held back completion payments by provider


                    var providerSummaryModel = providerSummary.GetMetrics();
                    providerSummaries.Add(providerSummaryModel);
                }

                periodEndSummary.AddProviderSummaries(providerSummaries);

                var overallPeriodEndSummary = periodEndSummary.GetMetrics();

                stopwatch.Stop();
                var dataDuration = stopwatch.ElapsedMilliseconds;
                //log duration

                await periodEndMetricsRepository.SaveProviderSummaries(providerSummaries, cancellationToken);
                await periodEndMetricsRepository.SavePeriodEndSummary(overallPeriodEndSummary, cancellationToken);

                logger.LogInfo(
                    $"Finished building period end metrics for {academicYear}, {collectionPeriod} using job id {jobId}, DataDuration: {dataDuration} milliseconds");
            }
            catch (Exception e)
            {
                logger.LogWarning(
                    $"Error building period end metrics for {academicYear}, {collectionPeriod} using job id {jobId}. Error: {e}");
                throw;
            }
        }
    }
}