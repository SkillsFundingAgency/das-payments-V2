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
using SFA.DAS.Payments.FundingSource.Application.Infrastructure;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public class RequiredLevyAmountFundingSourceService : IRequiredLevyAmountFundingSourceService
    {
        private readonly IPaymentProcessor processor;
        private readonly IMapper mapper;
        private readonly IDataCache<CalculatedRequiredLevyAmount> requiredPaymentsCache;
        private readonly IDataCache<List<string>> requiredPaymentKeys;
        private readonly ILevyAccountRepository levyAccountRepository;
        private readonly ILevyBalanceService levyBalanceService;
        private readonly IPaymentLogger paymentLogger;
        private readonly ISortableKeyGenerator sortableKeys;
        private readonly IDataCache<bool> monthEndCache;
        private readonly IDataCache<LevyAccountModel> levyAccountCache;
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
            IDataCache<bool> monthEndCache,
            IDataCache<LevyAccountModel> levyAccountCache,
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
            this.monthEndCache = monthEndCache ?? throw new ArgumentNullException(nameof(monthEndCache));
            this.levyAccountCache = levyAccountCache ?? throw new ArgumentNullException(nameof(levyAccountCache));
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
            await requiredPaymentKeys.AddOrReplace(CacheKeys.KeyListKey, keys).ConfigureAwait(false);
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
            await requiredPaymentKeys.AddOrReplace(CacheKeys.KeyListKey, keys, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<ReadOnlyCollection<FundingSourcePaymentEvent>> ProcessReceiverTransferPayment(ProcessUnableToFundTransferFundingSourcePayment message)
        {
            if (!message.AccountId.HasValue)
                throw new InvalidOperationException($"Invalid ProcessUnableToFundTransferFundingSourcePayment event.  No account id populated on message.  Event id: {message.EventId}");

            paymentLogger.LogDebug($"Converting the unable to fund transfer payment to a levy payment.  Event id: {message.EventId}, account id: {message.AccountId}, job id: {message.JobId}");
            var requiredPayment = mapper.Map<CalculatedRequiredLevyAmount>(message);
            paymentLogger.LogVerbose($"Mapped ProcessUnableToFundTransferFundingSourcePayment to CalculatedRequiredLevyAmount");
            var payments = new List<FundingSourcePaymentEvent>();
            var monthEndStartedCacheItem = await monthEndCache.TryGet(CacheKeys.MonthEndCacheKey);
            if (!monthEndStartedCacheItem.HasValue || !monthEndStartedCacheItem.Value)
            {
                paymentLogger.LogDebug($"Month end has not been started yet so adding the payment to the cache.");
                await AddRequiredPayment(requiredPayment);
            }
            else
            {
                var levyAccountCacheItem = await levyAccountCache.TryGet(CacheKeys.LevyBalanceKey, CancellationToken.None)
                    .ConfigureAwait(false);
                if (!levyAccountCacheItem.HasValue)
                    throw new InvalidOperationException($"The last levy account balance has not been stored in the reliable for account: {message.AccountId}");

                levyBalanceService.Initialise(levyAccountCacheItem.Value.Balance, levyAccountCacheItem.Value.TransferAllowance);
                paymentLogger.LogDebug($"Service has finished month end processing so now generating the payments for the ProcessUnableToFundTransferFundingSourcePayment event.");
                payments.AddRange(CreateFundingSourcePaymentsForRequiredPayment(requiredPayment, message.AccountId.Value, message.JobId));
                var remainingBalance = mapper.Map<LevyAccountModel>(levyAccountCacheItem.Value);
                remainingBalance.Balance = levyBalanceService.RemainingBalance;
                remainingBalance.TransferAllowance = levyBalanceService.RemainingTransferAllowance;
                await levyAccountCache.AddOrReplace(CacheKeys.LevyBalanceKey, remainingBalance);
            }
            paymentLogger.LogInfo($"Finished processing the ProcessUnableToFundTransferFundingSourcePayment. Event id: {message.EventId}, account id: {message.AccountId}, job id: {message.JobId}");
            return payments.AsReadOnly();
        }


        public async Task<ReadOnlyCollection<FundingSourcePaymentEvent>> HandleMonthEnd(long employerAccountId, long jobId)
        {
            var fundingSourceEvents = new List<FundingSourcePaymentEvent>();

            var keys = await GetKeys().ConfigureAwait(false);
            keys.Sort();

            var levyAccount = await levyAccountRepository.GetLevyAccount(employerAccountId);
            levyBalanceService.Initialise(levyAccount.Balance, levyAccount.TransferAllowance);

            paymentLogger.LogDebug($"Processing {keys.Count} required payments, levy balance {levyAccount.Balance}, account {employerAccountId}, job id {jobId}");

            foreach (var key in keys)
            {
                var requiredPaymentEvent = await requiredPaymentsCache.TryGet(key).ConfigureAwait(false);
                fundingSourceEvents.AddRange(CreateFundingSourcePaymentsForRequiredPayment(requiredPaymentEvent.Value, employerAccountId, jobId));
                await requiredPaymentsCache.Clear(key).ConfigureAwait(false);
            }

            paymentLogger.LogDebug($"Created {fundingSourceEvents.Count} payments - {GetFundsDebugString(fundingSourceEvents)}, account {employerAccountId}, job id {jobId}");

            levyAccount.Balance = levyBalanceService.RemainingBalance;
            levyAccount.TransferAllowance = levyBalanceService.RemainingTransferAllowance;
            await levyAccountCache.AddOrReplace(CacheKeys.LevyBalanceKey, levyAccount);

            await requiredPaymentKeys.Clear(CacheKeys.KeyListKey).ConfigureAwait(false);
            await monthEndCache.AddOrReplace(CacheKeys.MonthEndCacheKey, true, CancellationToken.None);
            paymentLogger.LogInfo($"Finished generating levy and/or co-invested payments for the account: {employerAccountId}, number of payments: {fundingSourceEvents.Count}.");
            return fundingSourceEvents.AsReadOnly();
        }

        private List<FundingSourcePaymentEvent> CreateFundingSourcePaymentsForRequiredPayment(CalculatedRequiredLevyAmount requiredPaymentEvent, long employerAccountId, long jobId)
        {
            var fundingSourceEvents = new List<FundingSourcePaymentEvent>();
            var requiredPayment = new RequiredPayment
            {
                SfaContributionPercentage = requiredPaymentEvent.SfaContributionPercentage,
                AmountDue = requiredPaymentEvent.AmountDue,
                IsTransfer = employerAccountId != requiredPaymentEvent.AccountId
                             && requiredPaymentEvent.TransferSenderAccountId.HasValue
                             && requiredPaymentEvent.TransferSenderAccountId == employerAccountId
            };

            var fundingSourcePayments = processor.Process(requiredPayment);
            foreach (var fundingSourcePayment in fundingSourcePayments)
            {
                var fundingSourceEvent = mapper.Map<FundingSourcePaymentEvent>(fundingSourcePayment);
                mapper.Map(requiredPaymentEvent, fundingSourceEvent);
                fundingSourceEvent.JobId = jobId;
                fundingSourceEvents.Add(fundingSourceEvent);
            }

            return fundingSourceEvents;
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
            var keysValue = await requiredPaymentKeys.TryGet(CacheKeys.KeyListKey).ConfigureAwait(false);
            var keys = keysValue.HasValue ? keysValue.Value : new List<string>();
            return keys;
        }
    }
}