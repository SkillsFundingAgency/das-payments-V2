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
                var currentPaymentTotals =
                    periodEndMetricsRepository.GetYearToDatePayments(academicYear, collectionPeriod, cancellationToken);

                //get data-locked amounts by provider
                var dataLockedEarningsTask =
                    periodEndMetricsRepository.GetDataLockedEarningsTotals(academicYear, collectionPeriod,
                        cancellationToken);
                var dataLockedAlreadyPaidTask =
                    periodEndMetricsRepository.GetAlreadyPaidDataLockedEarnings(academicYear, collectionPeriod,
                        cancellationToken);
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

                    //DC earnings YTD
                    providerSummary.AddDcEarning(dcEarningsTask.Result.Where(x => x.Ukprn == ukprn));
                    //payment this collection period by transactiontype/contractype
                    providerSummary.AddTransactionTypes(
                        transactionTypesTask.Result.Where(x => x.Ukprn == ukprn));
                    ////payment this collection period by funding source per contract type
                    providerSummary.AddFundingSourceAmounts(fundingSourceTask.Result.Where(x => x.Ukprn == ukprn));
                    //payments year to date prior to collection period in event
                    providerSummary.AddPaymentsYearToDate(currentPaymentTotals.Result.Where(x => x.Ukprn == ukprn));
                    //Total Datalocked using last successful jobs
                    providerSummary.AddDataLockedEarnings(
                        dataLockedEarningsTask.Result.FirstOrDefault(x => x.Ukprn == ukprn)?.TotalAmount ?? 0m);
                    //total datalocked already paid using last successful job
                    providerSummary.AddDataLockedAlreadyPaidTask(
                        dataLockedAlreadyPaidTask.Result.FirstOrDefault(x => x.Ukprn == ukprn)?.TotalAmount ?? 0m);
                    //add held back completion payments by provider
                    providerSummary.AddHeldBackCompletionPayments(
                        heldBackCompletionAmountsTask.Result.Where(x => x.Ukprn == ukprn));

                    var providerSummaryModel = providerSummary.GetMetrics();
                    providerSummaries.Add(providerSummaryModel);
                }

                periodEndSummary.AddProviderSummaries(providerSummaries);

                var overallPeriodEndSummary = periodEndSummary.GetMetrics();

                stopwatch.Stop();
                var dataDuration = stopwatch.ElapsedMilliseconds;
                //log duration
                
                await periodEndMetricsRepository.SaveProviderSummaries(providerSummaries, overallPeriodEndSummary,
                    cancellationToken);

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