using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Extensions;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public class RequiredLevyAmountFundingSourceService : IRequiredLevyAmountFundingSourceService
    {
        private const string KeyListKey = "keys";

        private readonly IPaymentProcessor processor;
        private readonly IMapper mapper;
        private readonly IDataCache<CalculatedRequiredLevyAmount> requiredPaymentsCache;
        private readonly IDataCache<List<string>> requiredPaymentKeys;
        private readonly ILevyAccountRepository levyAccountRepository;
        private readonly ILevyBalanceService levyBalanceService;
        private readonly IPaymentLogger paymentLogger;
        private readonly ISortableKeyGenerator sortableKeys;
        private readonly IDataCache<DateTime> submissionTimesCache;

        public RequiredLevyAmountFundingSourceService(
            IPaymentProcessor processor,
            IMapper mapper,
            IDataCache<CalculatedRequiredLevyAmount> requiredPaymentsCache,
            IDataCache<List<string>> requiredPaymentKeys,
            ILevyAccountRepository levyAccountRepository,
            ILevyBalanceService levyBalanceService,
            IPaymentLogger paymentLogger,
            ISortableKeyGenerator sortableKeys,
            IDataCache<DateTime> submissionTimesCache)
        {
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.requiredPaymentsCache = requiredPaymentsCache ?? throw new ArgumentNullException(nameof(requiredPaymentsCache));
            this.requiredPaymentKeys = requiredPaymentKeys ?? throw new ArgumentNullException(nameof(requiredPaymentKeys));
            this.levyAccountRepository = levyAccountRepository ?? throw new ArgumentNullException(nameof(levyAccountRepository));
            this.levyBalanceService = levyBalanceService ?? throw new ArgumentNullException(nameof(levyBalanceService));
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.sortableKeys = sortableKeys ?? throw new ArgumentNullException(nameof(sortableKeys));
            this.submissionTimesCache = submissionTimesCache ?? throw new ArgumentNullException(nameof(submissionTimesCache));
        }

        public async Task IlrSubmitted(ReceivedProviderEarningsEvent message)
        {
            var keys = await GetKeys().ConfigureAwait(false);
            var cachedKeys = keys.ToList();
            foreach (var key in cachedKeys)
            {
                var paymentCacheItem = await requiredPaymentsCache.TryGet(key, CancellationToken.None);
                if (!paymentCacheItem.HasValue)
                {
                    paymentLogger.LogWarning($"Payment not found for key: {key}");
                    continue;
                }

                var payment = paymentCacheItem.Value;
                if (payment.Ukprn != message.Ukprn || payment.IlrSubmissionDateTime >= message.IlrSubmissionDateTime)
                    continue;
                await requiredPaymentsCache.Clear(key).ConfigureAwait(false);
                keys.Remove(key);
            }
            await requiredPaymentKeys.AddOrReplace(KeyListKey, keys).ConfigureAwait(false);
            await submissionTimesCache.AddOrReplace($"provider_{message.Ukprn}", message.IlrSubmissionDateTime).ConfigureAwait(false);
        }

        private static string GetSubmissionTimeKey(long ukprn) => $"provider_{ukprn}";

        public async Task AddRequiredPayment(CalculatedRequiredLevyAmount paymentEvent)
        {
            var ilrSubmissionTimeCacheItem = await submissionTimesCache.TryGet(GetSubmissionTimeKey(paymentEvent.Ukprn), CancellationToken.None)
                .ConfigureAwait(false);
            if (!ilrSubmissionTimeCacheItem.HasValue)
                await submissionTimesCache.AddOrReplace($"provider_{paymentEvent.Ukprn}", paymentEvent.IlrSubmissionDateTime, CancellationToken.None)
                    .ConfigureAwait(false);
            else
            {
                if (paymentEvent.IlrSubmissionDateTime < ilrSubmissionTimeCacheItem.Value)
                {
                    paymentLogger.LogWarning($"Received out of sequence payment.  Current submission time: {ilrSubmissionTimeCacheItem.Value}, payment submission time: {paymentEvent.IlrSubmissionDateTime}");
                    return;
                }
            }
            var keys = await GetKeys().ConfigureAwait(false);
            var key = sortableKeys.Generate(paymentEvent.AmountDue, paymentEvent.Priority,
                paymentEvent.Learner.Uln, paymentEvent.StartDate, paymentEvent.IsTransfer(), paymentEvent.EventId);
            keys.Add(key);
            await requiredPaymentsCache.Add(key, paymentEvent, CancellationToken.None).ConfigureAwait(false);
            await requiredPaymentKeys.AddOrReplace(KeyListKey, keys, CancellationToken.None).ConfigureAwait(false);
        }


        public async Task<ReadOnlyCollection<FundingSourcePaymentEvent>> GetFundedPayments(long employerAccountId, long jobId)
        {
            var fundingSourceEvents = new List<FundingSourcePaymentEvent>();

            var keys = await GetKeys().ConfigureAwait(false);
            if (keys.Count == 0)
                return fundingSourceEvents.AsReadOnly();

            keys.Sort();

            var levyAccount = await levyAccountRepository.GetLevyAccount(employerAccountId);
            levyBalanceService.Initialise(levyAccount.Balance, levyAccount.TransferAllowance);

            paymentLogger.LogDebug($"Processing {keys.Count} required payments, levy balance {levyAccount.Balance}, account {employerAccountId}, job id {jobId}");

            foreach (var key in keys)
            {
                var requiredPaymentEvent = await requiredPaymentsCache.TryGet(key).ConfigureAwait(false);
                var requiredPayment = new RequiredPayment
                {
                    SfaContributionPercentage = requiredPaymentEvent.Value.SfaContributionPercentage,
                    AmountDue = requiredPaymentEvent.Value.AmountDue,
                    IsTransfer = employerAccountId != requiredPaymentEvent.Value.AccountId
                                 && requiredPaymentEvent.Value.TransferSenderAccountId.HasValue
                                 && requiredPaymentEvent.Value.TransferSenderAccountId == employerAccountId
                };

                var fundingSourcePayments = processor.Process(requiredPayment);
                foreach (var fundingSourcePayment in fundingSourcePayments)
                {
                    var fundingSourceEvent = mapper.Map<FundingSourcePaymentEvent>(fundingSourcePayment);
                    mapper.Map(requiredPaymentEvent.Value, fundingSourceEvent);
                    fundingSourceEvent.JobId = jobId;
                    fundingSourceEvents.Add(fundingSourceEvent);
                }

                await requiredPaymentsCache.Clear(key).ConfigureAwait(false);
            }

            paymentLogger.LogDebug($"Created {fundingSourceEvents.Count} payments - {GetFundsDebugString(fundingSourceEvents)}, account {employerAccountId}, job id {jobId}");

            await requiredPaymentKeys.Clear(KeyListKey).ConfigureAwait(false);
            return fundingSourceEvents.AsReadOnly();
        }

        private static string GetFundsDebugString(List<FundingSourcePaymentEvent> fundingSourceEvents)
        {
            var fundsGroupedBySource = fundingSourceEvents.GroupBy(f => f.FundingSourceType);
            var debugStrings = fundsGroupedBySource.Select(group => string.Concat(group.Key, ": ", ConcatAmounts(group)));
            return string.Join(", ", debugStrings);

            string ConcatAmounts(IEnumerable<FundingSourcePaymentEvent> funds)
            {
                return string.Join("+", funds.Select(f => f.AmountDue.ToString("#,##0.##")));
            }
        }

        private async Task<List<string>> GetKeys()
        {
            var keysValue = await requiredPaymentKeys.TryGet(KeyListKey).ConfigureAwait(false);
            var keys = keysValue.HasValue ? keysValue.Value : new List<string>();
            return keys;
        }
    }
}