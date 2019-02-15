using System;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class MonthEndService : IMonthEndService
    {
        private readonly IProviderPaymentsRepository providerPaymentsRepository;
        private readonly IMonthEndCache monthEndCache;
        private readonly IPaymentLogger logger;

        public MonthEndService(IProviderPaymentsRepository providerPaymentsRepository, IMonthEndCache monthEndCache, IPaymentLogger logger)
        {
            this.providerPaymentsRepository = providerPaymentsRepository;
            this.monthEndCache = monthEndCache ?? throw new ArgumentNullException(nameof(monthEndCache));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<PaymentModel>> GetMonthEndPayments(CollectionPeriod collectionPeriod,
            long ukprn,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await providerPaymentsRepository.GetMonthEndPayments(collectionPeriod, ukprn, cancellationToken);
        }

        public async Task StartMonthEnd(long ukprn, short academicYear, byte collectionPeriod)
        {
            logger.LogVerbose($"Recoding month end in the cache. Ukprn: {ukprn}, academic year: {academicYear}, collection period: {collectionPeriod}");
            await monthEndCache.AddOrReplace(ukprn, academicYear, collectionPeriod);
            logger.LogDebug($"Recoded month end in the cache. Ukprn: {ukprn}, academic year: {academicYear}, collection period: {collectionPeriod}");
        }

        public Task<bool> MonthEndStarted(long ukprn, short academicYear, byte collectionPeriod)
        {
            throw new NotImplementedException();
        }
    }
}