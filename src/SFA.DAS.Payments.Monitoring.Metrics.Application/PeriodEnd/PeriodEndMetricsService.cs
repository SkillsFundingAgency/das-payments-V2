﻿using System;
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

                // - get DC earnings grouped by UKPRN 
                var dcEarningsTask = dcDataContext.GetEarnings(academicYear, collectionPeriod, cancellationToken);
                var transactionTypesByContractType =
                    periodEndMetricsRepository.GetTransactionTypesByContractType(academicYear, collectionPeriod,
                        cancellationToken);
                //get payments by provider by FundingSource per contract type
                var fundingSourceAmounts =
                    periodEndMetricsRepository.GetFundingSourceAmountsByContractType(academicYear, collectionPeriod,
                        cancellationToken);
                //get payments totals by provider per contract type
                //might not need this???
                //get data-locked amounts by provider
                var dataLockedAmountTask =
                    periodEndMetricsRepository.GetDataLockedAmounts(academicYear, collectionPeriod, cancellationToken);
                //get held back completion payments by provider

                var dataTask = Task.WhenAll(dcEarningsTask, transactionTypesByContractType, fundingSourceAmounts);
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
                    //payment by provider /transactiontype/contractype
                    providerSummary.AddTransactionTypes(
                        transactionTypesByContractType.Result.Where(x => x.Ukprn == ukprn));
                    //add payments by provider by funding source per contract type
                    providerSummary.AddFundingSourceAmounts(fundingSourceAmounts.Result.Where(x => x.Ukprn == ukprn));
                    //add payments totals by provider per contract type
                    //might not need this???
                    //add data-locked amounts by provider 

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
                    $"Finished building period end metrics for {academicYear}, {collectionPeriod} using job id {jobId}");
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