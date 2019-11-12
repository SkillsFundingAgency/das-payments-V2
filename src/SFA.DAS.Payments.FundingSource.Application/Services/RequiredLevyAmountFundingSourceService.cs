using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Extensions;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.FundingSource.Model;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public class RequiredLevyAmountFundingSourceService : IRequiredLevyAmountFundingSourceService
    {
        private readonly IPaymentProcessor processor;
        private readonly IMapper mapper;
        private readonly IDataCache<CalculatedRequiredLevyAmount> requiredPaymentsCache;
        private readonly ILevyFundingSourceRepository levyFundingSourceRepository;
        private readonly ILevyBalanceService levyBalanceService;
        private readonly IPaymentLogger paymentLogger;
        private readonly IDataCache<bool> monthEndCache;
        private readonly IDataCache<LevyAccountModel> levyAccountCache;
        private readonly IDataCache<List<EmployerProviderPriorityModel>> employerProviderPriorities;
        private readonly IDataCache<List<string>> refundSortKeysCache;
        private readonly IDataCache<List<TransferPaymentSortKeyModel>> transferPaymentSortKeysCache;
        private readonly IDataCache<List<RequiredPaymentSortKeyModel>> requiredPaymentSortKeysCache;
        private readonly IGenerateSortedPaymentKeys generateSortedPaymentKeys;

        public RequiredLevyAmountFundingSourceService(
            IPaymentProcessor processor,
            IMapper mapper,
            IDataCache<CalculatedRequiredLevyAmount> requiredPaymentsCache,
            ILevyFundingSourceRepository levyFundingSourceRepository,
            ILevyBalanceService levyBalanceService,
            IPaymentLogger paymentLogger,
            IDataCache<bool> monthEndCache,
            IDataCache<LevyAccountModel> levyAccountCache,
            IDataCache<List<EmployerProviderPriorityModel>> employerProviderPriorities,
            IDataCache<List<string>> refundSortKeysCache,
            IDataCache<List<TransferPaymentSortKeyModel>> transferPaymentSortKeysCache,
            IDataCache<List<RequiredPaymentSortKeyModel>> requiredPaymentSortKeysCache,
            IGenerateSortedPaymentKeys generateSortedPaymentKeys)
        {
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.requiredPaymentsCache = requiredPaymentsCache ?? throw new ArgumentNullException(nameof(requiredPaymentsCache));
            this.levyFundingSourceRepository = levyFundingSourceRepository ?? throw new ArgumentNullException(nameof(levyFundingSourceRepository));
            this.levyBalanceService = levyBalanceService ?? throw new ArgumentNullException(nameof(levyBalanceService));
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.monthEndCache = monthEndCache ?? throw new ArgumentNullException(nameof(monthEndCache));
            this.levyAccountCache = levyAccountCache ?? throw new ArgumentNullException(nameof(levyAccountCache));
            this.employerProviderPriorities = employerProviderPriorities ?? throw new ArgumentNullException(nameof(employerProviderPriorities));
            this.refundSortKeysCache = refundSortKeysCache ?? throw new ArgumentNullException(nameof(refundSortKeysCache));
            this.transferPaymentSortKeysCache = transferPaymentSortKeysCache ?? throw new ArgumentNullException(nameof(transferPaymentSortKeysCache));
            this.requiredPaymentSortKeysCache = requiredPaymentSortKeysCache ?? throw new ArgumentNullException(nameof(requiredPaymentSortKeysCache));
            this.generateSortedPaymentKeys = generateSortedPaymentKeys ?? throw new ArgumentNullException(nameof(generateSortedPaymentKeys));
        }

        public async Task AddRequiredPayment(CalculatedRequiredLevyAmount paymentEvent)
        {
            if (paymentEvent.AmountDue < 0)
            {
                await AddRefundPaymentToCache(paymentEvent);
                return;
            }

            if (paymentEvent.IsTransfer())
            {
                await AddTransferPaymentToCache(paymentEvent);
                return;
            }

            await AddRequiredPaymentToCache(paymentEvent);
        }

        public async Task StoreEmployerProviderPriority(EmployerChangedProviderPriority providerPriorityEvent)
        {
            try
            {
                int order = 1;
                var paymentPriorities = new List<EmployerProviderPriorityModel>();
                foreach (var providerUkprn in providerPriorityEvent.OrderedProviders)
                {
                    paymentPriorities.Add(new EmployerProviderPriorityModel
                    {
                        Ukprn = providerUkprn,
                        EmployerAccountId = providerPriorityEvent.EmployerAccountId,
                        Order = order
                    });

                    order++;
                }

                using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                {
                    paymentLogger.LogDebug($"Replacing Previous EmployerProviderPriority for Account Id {providerPriorityEvent.EmployerAccountId}");
                    await levyFundingSourceRepository.ReplaceEmployerProviderPriorities(providerPriorityEvent.EmployerAccountId, paymentPriorities).ConfigureAwait(false);
                    paymentLogger.LogDebug($"Successfully Replaced Previous EmployerProviderPriority for Account Id {providerPriorityEvent.EmployerAccountId}");

                    paymentLogger.LogDebug($"Adding EmployerProviderPriority to Cache for Account Id {providerPriorityEvent.EmployerAccountId}");
                    await employerProviderPriorities.AddOrReplace(CacheKeys.EmployerPaymentPriorities, paymentPriorities).ConfigureAwait(false);
                    paymentLogger.LogInfo($"Successfully Add EmployerProviderPriority to Cache for Account Id {providerPriorityEvent.EmployerAccountId}");

                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                paymentLogger.LogError($"Error while updating  EmployerProviderPriority for Account Id {providerPriorityEvent.EmployerAccountId}", e);
                throw;
            }

        }

        public async Task<ReadOnlyCollection<FundingSourcePaymentEvent>> ProcessReceiverTransferPayment(ProcessUnableToFundTransferFundingSourcePayment message)
        {
            if (!message.AccountId.HasValue)
                throw new InvalidOperationException($"Invalid ProcessUnableToFundTransferFundingSourcePayment event.  No account id populated on message.  Event id: {message.EventId}");

            paymentLogger.LogDebug($"Converting the unable to fund transfer payment to a levy payment.  Event id: {message.EventId}, account id: {message.AccountId}, job id: {message.JobId}");
            var requiredPayment = mapper.Map<CalculatedRequiredLevyAmount>(message);
            paymentLogger.LogVerbose("Mapped ProcessUnableToFundTransferFundingSourcePayment to CalculatedRequiredLevyAmount");
            var payments = new List<FundingSourcePaymentEvent>();
            var monthEndStartedCacheItem = await monthEndCache.TryGet(CacheKeys.MonthEndCacheKey);
            if (!monthEndStartedCacheItem.HasValue || !monthEndStartedCacheItem.Value)
            {
                paymentLogger.LogDebug("Month end has not been started yet so adding the payment to the cache.");
                await AddRequiredPayment(requiredPayment);
            }
            else
            {
                var levyAccountCacheItem = await levyAccountCache.TryGet(CacheKeys.LevyBalanceKey, CancellationToken.None)
                    .ConfigureAwait(false);
                if (!levyAccountCacheItem.HasValue)
                    throw new InvalidOperationException($"The last levy account balance has not been stored in the reliable for account: {message.AccountId}");

                levyBalanceService.Initialise(levyAccountCacheItem.Value.Balance, levyAccountCacheItem.Value.TransferAllowance);
                paymentLogger.LogDebug("Service has finished month end processing so now generating the payments for the ProcessUnableToFundTransferFundingSourcePayment event.");
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

            var keys = await generateSortedPaymentKeys.GeyKeys().ConfigureAwait(false);

            var levyAccount = await levyFundingSourceRepository.GetLevyAccount(employerAccountId);
            levyBalanceService.Initialise(levyAccount.Balance, levyAccount.TransferAllowance);

            paymentLogger.LogDebug($"Processing {keys.Count} required payments, levy balance {levyAccount.Balance}, account {employerAccountId}, job id {jobId}");

            foreach (var key in keys)
            {
                var requiredPaymentEvent = await requiredPaymentsCache.TryGet(key).ConfigureAwait(false);
                if (!requiredPaymentEvent.HasValue)
                    continue;
                fundingSourceEvents.AddRange(CreateFundingSourcePaymentsForRequiredPayment(requiredPaymentEvent.Value, employerAccountId, jobId));
                await requiredPaymentsCache.Clear(key).ConfigureAwait(false);
            }

            paymentLogger.LogDebug($"Created {fundingSourceEvents.Count} payments - {GetFundsDebugString(fundingSourceEvents)}, account {employerAccountId}, job id {jobId}");

            levyAccount.Balance = levyBalanceService.RemainingBalance;
            levyAccount.TransferAllowance = levyBalanceService.RemainingTransferAllowance;
            await levyAccountCache.AddOrReplace(CacheKeys.LevyBalanceKey, levyAccount);

            await refundSortKeysCache.Clear(CacheKeys.RefundPaymentsKeyListKey).ConfigureAwait(false);
            await transferPaymentSortKeysCache.Clear(CacheKeys.SenderTransferKeyListKey).ConfigureAwait(false);
            await requiredPaymentSortKeysCache.Clear(CacheKeys.RequiredPaymentKeyListKey).ConfigureAwait(false);

            await monthEndCache.AddOrReplace(CacheKeys.MonthEndCacheKey, true, CancellationToken.None);
            paymentLogger.LogInfo($"Finished generating levy and/or co-invested payments for the account: {employerAccountId}, number of payments: {fundingSourceEvents.Count}.");
            return fundingSourceEvents.AsReadOnly();
        }

        public async Task RemovePreviousSubmissions(long jobId, byte collectionPeriod, short academicYear,
            DateTime submissionDate, long ukprn)
        {
            var keys = await generateSortedPaymentKeys.GeyKeys().ConfigureAwait(false);

            paymentLogger.LogDebug($"Processing {keys.Count} required payments, job id {jobId}");

            foreach (var key in keys)
            {
                var cacheItem = await requiredPaymentsCache.TryGet(key).ConfigureAwait(false);
                if (!cacheItem.HasValue)
                    continue;

                var requiredPaymentEvent = cacheItem.Value;

                if (requiredPaymentEvent.CollectionPeriod.AcademicYear == academicYear &&
                    requiredPaymentEvent.CollectionPeriod.Period == collectionPeriod &&
                    requiredPaymentEvent.JobId != jobId &&
                    requiredPaymentEvent.IlrSubmissionDateTime < submissionDate &&
                    requiredPaymentEvent.Ukprn == ukprn)
                {
                    await requiredPaymentsCache.Clear(key).ConfigureAwait(false);
                }
            }
            paymentLogger.LogInfo("Finished removing previous submission payments.");
        }

        public async Task RemoveCurrentSubmission(long jobId, byte collectionPeriod, short academicYear,
            DateTime submissionDate, long ukprn)
        {
            var keys = await generateSortedPaymentKeys.GeyKeys().ConfigureAwait(false);

            paymentLogger.LogDebug($"Processing {keys.Count} required payments, job id {jobId}");
            foreach (var key in keys)
            {
                var cacheItem = await requiredPaymentsCache.TryGet(key).ConfigureAwait(false);
                if (!cacheItem.HasValue)
                    continue;

                var requiredPaymentEvent = cacheItem.Value;

                if (requiredPaymentEvent.CollectionPeriod.AcademicYear == academicYear &&
                    requiredPaymentEvent.CollectionPeriod.Period == collectionPeriod &&
                    requiredPaymentEvent.JobId == jobId && 
                    requiredPaymentEvent.Ukprn == ukprn)
                {
                    await requiredPaymentsCache.Clear(key).ConfigureAwait(false);
                }
            }
            paymentLogger.LogInfo("Finished removing current submission payments.");
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

        private async Task AddRefundPaymentToCache(CalculatedRequiredLevyAmount paymentEvent)
        {
            var keysValue = await refundSortKeysCache.TryGet(CacheKeys.RefundPaymentsKeyListKey).ConfigureAwait(false);
            var refundKeysList = keysValue.HasValue ? keysValue.Value : new List<string>();

            refundKeysList.Add(paymentEvent.EventId.ToString());
            await requiredPaymentsCache.AddOrReplace(paymentEvent.EventId.ToString(), paymentEvent).ConfigureAwait(false);
            await refundSortKeysCache.AddOrReplace(CacheKeys.RefundPaymentsKeyListKey, refundKeysList).ConfigureAwait(false);
        }

        private async Task AddTransferPaymentToCache(CalculatedRequiredLevyAmount paymentEvent)
        {
            var keysValue = await transferPaymentSortKeysCache.TryGet(CacheKeys.SenderTransferKeyListKey)
                .ConfigureAwait(false);
            var transferKeysList = keysValue.HasValue ? keysValue.Value : new List<TransferPaymentSortKeyModel>();

            var newTransferKey = new TransferPaymentSortKeyModel
            {
                Id = paymentEvent.EventId.ToString(),
                Uln = paymentEvent.Learner.Uln,
                AgreedOnDate = paymentEvent.AgreedOnDate ?? DateTime.MinValue
            };

            transferKeysList.Add(newTransferKey);

            await requiredPaymentsCache.AddOrReplace(newTransferKey.Id, paymentEvent).ConfigureAwait(false);
            await transferPaymentSortKeysCache.AddOrReplace(CacheKeys.SenderTransferKeyListKey, transferKeysList).ConfigureAwait(false);
        }

        private async Task AddRequiredPaymentToCache(CalculatedRequiredLevyAmount paymentEvent)
        {
            var keysValue = await requiredPaymentSortKeysCache.TryGet(CacheKeys.RequiredPaymentKeyListKey).ConfigureAwait(false);
            var requiredPaymentKeysList = keysValue.HasValue ? keysValue.Value : new List<RequiredPaymentSortKeyModel>();

            var newRequiredPaymentSortKey = new RequiredPaymentSortKeyModel
            {
                Id = paymentEvent.EventId.ToString(),
                Uln = paymentEvent.Learner.Uln,
                Ukprn = paymentEvent.Ukprn,
                AgreedOnDate = paymentEvent.AgreedOnDate ?? DateTime.MinValue
            };

            requiredPaymentKeysList.Add(newRequiredPaymentSortKey);

            await requiredPaymentsCache.AddOrReplace(newRequiredPaymentSortKey.Id, paymentEvent).ConfigureAwait(false);
            await requiredPaymentSortKeysCache.AddOrReplace(CacheKeys.RequiredPaymentKeyListKey, requiredPaymentKeysList).ConfigureAwait(false);
        }

    }
}