using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public class ContractType1RequiredPaymentEventFundingSourceService : IContractType1RequiredPaymentEventFundingSourceService
    {
        private const string KeyListKey = "keys";

        private readonly IEnumerable<IPaymentProcessor> processors;
        private readonly IMapper mapper;
        private readonly IDataCache<ApprenticeshipContractType1RequiredPaymentEvent> requiredPaymentsCache;
        private readonly IDataCache<List<string>> requiredPaymentKeys;
        private readonly ILevyAccountRepository levyAccountRepository;
        private readonly long employerAccountId;

        public ContractType1RequiredPaymentEventFundingSourceService(
            IEnumerable<IPaymentProcessor> processors, 
            IMapper mapper, 
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
            var keys = await GetKeys().ConfigureAwait(false);
            var fundingSourceEvents = new List<FundingSourcePaymentEvent>();

            foreach (var key in keys)
            {
                var requiredPaymentEvent = await requiredPaymentsCache.TryGet(key).ConfigureAwait(false);
                var requiredPayment = new RequiredLevyPayment
                {
                    SfaContributionPercentage = requiredPaymentEvent.Value.SfaContributionPercentage,
                    AmountDue = requiredPaymentEvent.Value.AmountDue,
                    AmountFunded = requiredPaymentEvent.Value.AmountDue,
                    LevyBalance = levyAccount.Balance
                };

                foreach (var processor in processors)
                {
                    var fundingSourcePayment = processor.Process(requiredPayment);
                    if (fundingSourcePayment != null)
                    {
                        var fundingSourcePaymentEvent = mapper.Map<FundingSourcePaymentEvent>(fundingSourcePayment);
                        mapper.Map(requiredPaymentEvent.Value, fundingSourcePaymentEvent);
                        fundingSourceEvents.Add(fundingSourcePaymentEvent);
                    }

                    await requiredPaymentsCache.Clear(key).ConfigureAwait(false);
                }

                levyAccount.Balance = requiredPayment.LevyBalance;
            }

            await requiredPaymentKeys.Clear(KeyListKey).ConfigureAwait(false);

            // TODO: store new account levy balance

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