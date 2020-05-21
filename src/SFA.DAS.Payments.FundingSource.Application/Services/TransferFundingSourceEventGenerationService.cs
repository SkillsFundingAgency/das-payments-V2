using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public class TransferFundingSourceEventGenerationService : ITransferFundingSourceEventGenerationService
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IMapper mapper;
        private readonly IDataCache<bool> monthEndCache;
        private readonly IDataCache<LevyAccountModel> levyAccountCache;
        private readonly ILevyBalanceService levyBalanceService;
        private readonly IFundingSourcePaymentEventBuilder fundingSourcePaymentEventBuilder;
        private readonly ILevyTransactionBatchStorageService levyTransactionBatchStorageService;

        public TransferFundingSourceEventGenerationService(
            IPaymentLogger paymentLogger,
            IMapper mapper,
            IDataCache<bool> monthEndCache,
            IDataCache<LevyAccountModel> levyAccountCache,
            ILevyBalanceService levyBalanceService,
            IFundingSourcePaymentEventBuilder fundingSourcePaymentEventBuilder,
            ILevyTransactionBatchStorageService levyTransactionBatchStorageService
        )
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.monthEndCache = monthEndCache ?? throw new ArgumentNullException(nameof(monthEndCache));
            this.levyAccountCache = levyAccountCache ?? throw new ArgumentNullException(nameof(levyAccountCache));
            this.levyBalanceService = levyBalanceService ?? throw new ArgumentNullException(nameof(levyBalanceService));
            this.fundingSourcePaymentEventBuilder = fundingSourcePaymentEventBuilder ?? throw new ArgumentNullException(nameof(fundingSourcePaymentEventBuilder));
            this.levyTransactionBatchStorageService = levyTransactionBatchStorageService ?? throw new ArgumentNullException(nameof(levyTransactionBatchStorageService));
        }

        public async Task<ReadOnlyCollection<FundingSourcePaymentEvent>> ProcessReceiverTransferPayment(ProcessUnableToFundTransferFundingSourcePayment message)
        {
            if (!message.AccountId.HasValue)
                throw new InvalidOperationException($"Invalid ProcessUnableToFundTransferFundingSourcePayment event.  No account id populated on message.  Event id: {message.EventId}");

            paymentLogger.LogDebug($"Converting the unable to fund transfer payment to a levy payment.  Event id: {message.EventId}, account id: {message.AccountId}, job id: {message.JobId}");
            var requiredPayment = mapper.Map<CalculatedRequiredLevyAmount>(message);
            paymentLogger.LogVerbose("Mapped ProcessUnableToFundTransferFundingSourcePayment to CalculatedRequiredLevyAmount");
            var payments = new List<FundingSourcePaymentEvent>();
            var monthEndStartedForThisAccount = await monthEndCache.TryGet(CacheKeys.MonthEndStartedForThisAccountCacheKey);
            if (!monthEndStartedForThisAccount.HasValue || !monthEndStartedForThisAccount.Value)
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
                payments.AddRange(fundingSourcePaymentEventBuilder.BuildFundingSourcePaymentsForRequiredPayment(requiredPayment, message.AccountId.Value, message.JobId));
                var remainingBalance = mapper.Map<LevyAccountModel>(levyAccountCacheItem.Value);
                remainingBalance.Balance = levyBalanceService.RemainingBalance;
                remainingBalance.TransferAllowance = levyBalanceService.RemainingTransferAllowance;
                await levyAccountCache.AddOrReplace(CacheKeys.LevyBalanceKey, remainingBalance);
            }
            paymentLogger.LogInfo($"Finished processing the ProcessUnableToFundTransferFundingSourcePayment. Event id: {message.EventId}, account id: {message.AccountId}, job id: {message.JobId}");
            return payments.AsReadOnly();
        }

        private async Task AddRequiredPayment(CalculatedRequiredLevyAmount calculatedRequiredLevyAmount)
        {
            await levyTransactionBatchStorageService.StoreLevyTransactions(new List<CalculatedRequiredLevyAmount>{ calculatedRequiredLevyAmount }, CancellationToken.None, true);
        }
    }
}