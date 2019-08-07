using System;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class ProviderPeriodEndService : IProviderPeriodEndService
    {
        private readonly IProviderPaymentsRepository providerPaymentsRepository;
        private readonly IPaymentsEventModelBatchService<ProviderPaymentEventModel> batchService;
        private readonly IMonthEndCache monthEndCache;
        private readonly IPaymentLogger logger;

        public ProviderPeriodEndService(IProviderPaymentsRepository providerPaymentsRepository, IMonthEndCache monthEndCache, IPaymentLogger logger, IPaymentsEventModelBatchService<ProviderPaymentEventModel> batchService)
        {
            this.providerPaymentsRepository = providerPaymentsRepository;
            this.monthEndCache = monthEndCache ?? throw new ArgumentNullException(nameof(monthEndCache));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.batchService = batchService;
        }

        public async Task<List<PaymentModel>> GetMonthEndPayments(CollectionPeriod collectionPeriod, long ukprn, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await providerPaymentsRepository.GetMonthEndPayments(collectionPeriod, ukprn, cancellationToken).ConfigureAwait(false);
        }

        public async Task StartMonthEnd(long ukprn, short academicYear, byte collectionPeriod, long monthEndJobId)
        {
            logger.LogVerbose($"Recoding month end in the cache. Ukprn: {ukprn}, academic year: {academicYear}, collection period: {collectionPeriod}, Month End Job Id {monthEndJobId}");
            await monthEndCache.AddOrReplace(ukprn, academicYear, collectionPeriod, monthEndJobId).ConfigureAwait(false);
            logger.LogDebug($"Recoded month end in the cache. Ukprn: {ukprn}, academic year: {academicYear}, collection period: {collectionPeriod} , Month End Job Id {monthEndJobId}");

            logger.LogVerbose($"Flushing model cache. Ukprn: {ukprn}, academic year: {academicYear}, collection period: {collectionPeriod}, Month End Job Id {monthEndJobId}");
            await batchService.StorePayments(CancellationToken.None);
            logger.LogDebug($"Model cache flushed. Ukprn: {ukprn}, academic year: {academicYear}, collection period: {collectionPeriod} , Month End Job Id {monthEndJobId}");
        }

        public async Task<bool> MonthEndStarted(long ukprn, short academicYear, byte collectionPeriod)
        {
            return await monthEndCache.Exists( ukprn,  academicYear,  collectionPeriod).ConfigureAwait(false);
        }

        public async Task<long> GetMonthEndJobId(long ukprn, short academicYear, byte collectionPeriod)
        {
            var monthEndData =  await monthEndCache.GetMonthEndDetails(ukprn, academicYear, collectionPeriod).ConfigureAwait(false);
            return monthEndData.JobId;
        }
    }
}