using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public interface IContractType1RequiredPaymentEventFundingSourceService
    {
        Task RegisterRequiredPayment(ApprenticeshipContractType1RequiredPaymentEvent paymentEvent);
        Task<IReadOnlyCollection<FundingSourcePaymentEvent>> GetFundedPayments();
    }

    public class ContractType1RequiredPaymentEventFundingSourceService : IContractType1RequiredPaymentEventFundingSourceService
    {
        private const string KeyListKey = "keys";

        private readonly IEnumerable<ILevyPaymentProcessor> processors;
        private readonly ILevyFundingSourcePaymentEventMapper mapper;
        private readonly IDataCache<ApprenticeshipContractType1RequiredPaymentEvent> requiredPaymentsCache;
        private readonly IDataCache<List<string>> requiredPaymentKeys;
        private readonly ILevyAccountRepository levyAccountRepository;
        private readonly long employerAccountId;

        public ContractType1RequiredPaymentEventFundingSourceService(
            IEnumerable<ILevyPaymentProcessor> processors, 
            ILevyFundingSourcePaymentEventMapper mapper, 
            IDataCache<ApprenticeshipContractType1RequiredPaymentEvent> requiredPaymentsCache, 
            IDataCache<List<string>> requiredPaymentKeys, 
            ILevyAccountRepository levyAccountRepository, 
            long employerAccountId)
        {
            this.processors = processors ?? throw new ArgumentNullException(nameof(processors));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.requiredPaymentsCache = requiredPaymentsCache ?? throw new ArgumentNullException(nameof(requiredPaymentsCache));
            this.requiredPaymentKeys = requiredPaymentKeys ?? throw new ArgumentNullException(nameof(requiredPaymentKeys));
            this.levyAccountRepository = levyAccountRepository ?? throw new ArgumentNullException(nameof(levyAccountRepository));
            this.employerAccountId = employerAccountId;
        }

        public async Task RegisterRequiredPayment(ApprenticeshipContractType1RequiredPaymentEvent paymentEvent)
        {
            var keys = await GetKeys().ConfigureAwait(false);
            keys.Add(paymentEvent.EventId.ToString());
            await requiredPaymentsCache.Add(paymentEvent.EventId.ToString(), paymentEvent).ConfigureAwait(false);
            await requiredPaymentKeys.AddOrReplace(KeyListKey, keys).ConfigureAwait(false);
        }

        public async Task<IReadOnlyCollection<FundingSourcePaymentEvent>> GetFundedPayments()
        {
            var levyAccount = await levyAccountRepository.GetLevyAccount(employerAccountId);
            var balance = levyAccount.Balance;
            var keys = await GetKeys().ConfigureAwait(false);
            var fundingSourceEvents = new List<FundingSourcePaymentEvent>();

            foreach (var key in keys)
            {
                var requiredPaymentEvent = await requiredPaymentsCache.TryGet(key).ConfigureAwait(false);
                foreach (var processor in processors)
                {
                    var requiredPayment = new RequiredLevyPayment
                    {
                        AmountDue = requiredPaymentEvent.Value.AmountDue, 
                        SfaContributionPercentage = requiredPaymentEvent.Value.SfaContributionPercentage
                    };
                    var funding = processor.Process(requiredPayment, ref balance);
                    if (funding != null)
                        fundingSourceEvents.Add(mapper.MapToFundingSourcePaymentEvent(funding));

                    await requiredPaymentsCache.Clear(key).ConfigureAwait(false);
                }
            }

            await requiredPaymentKeys.Clear(KeyListKey).ConfigureAwait(false);

            return fundingSourceEvents;
        }

        private async Task<List<string>> GetKeys()
        {
            var keysValue = await requiredPaymentKeys.TryGet(KeyListKey).ConfigureAwait(false);
            var keys = keysValue.HasValue ? keysValue.Value : new List<string>();
            return keys;
        }
    }
}