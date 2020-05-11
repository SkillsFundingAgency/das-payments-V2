using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Data;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public class FundingSourceEventGenerationService : IFundingSourceEventGenerationService
    {
        private readonly IPaymentLogger logger;
        private readonly IFundingSourceDataContext dataContext;
        private readonly ILevyBalanceService levyBalanceService;
        private readonly ILevyFundingSourceRepository levyFundingSourceRepository;
        private readonly IDataCache<LevyAccountModel> levyAccountCache;
        private readonly ICalculatedRequiredLevyAmountPrioritisationService calculatedRequiredLevyAmountPrioritisationService;
        private readonly IFundingSourcePaymentEventBuilder fundingSourcePaymentEventBuilder;

        public FundingSourceEventGenerationService(
            IPaymentLogger logger,
            IFundingSourceDataContext dataContext,
            ILevyBalanceService levyBalanceService,
            ILevyFundingSourceRepository levyFundingSourceRepository,
            IDataCache<LevyAccountModel> levyAccountCache,
            ICalculatedRequiredLevyAmountPrioritisationService calculatedRequiredLevyAmountPrioritisationService,
            IFundingSourcePaymentEventBuilder fundingSourcePaymentEventBuilder
        ){
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.levyBalanceService = levyBalanceService ?? throw new ArgumentNullException(nameof(levyBalanceService));
            this.levyFundingSourceRepository = levyFundingSourceRepository ?? throw new ArgumentNullException(nameof(levyFundingSourceRepository));
            this.levyAccountCache = levyAccountCache ?? throw new ArgumentNullException(nameof(levyAccountCache));
            this.calculatedRequiredLevyAmountPrioritisationService = calculatedRequiredLevyAmountPrioritisationService ?? throw new ArgumentNullException(nameof(calculatedRequiredLevyAmountPrioritisationService));
            this.fundingSourcePaymentEventBuilder = fundingSourcePaymentEventBuilder ?? throw new ArgumentNullException(nameof(fundingSourcePaymentEventBuilder));
        }

        public async Task<ReadOnlyCollection<FundingSourcePaymentEvent>> HandleMonthEnd(long employerAccountId, long jobId)
        {
            var levyAccount = await levyFundingSourceRepository.GetLevyAccount(employerAccountId);
            levyBalanceService.Initialise(levyAccount.Balance, levyAccount.TransferAllowance);

            var orderedRequiredLevyPayments = await GetOrderedCalculatedRequiredLevyAmounts(employerAccountId).ConfigureAwait(false);

            logger.LogDebug($"Processing {orderedRequiredLevyPayments.Count} required payments, levy balance {levyAccount.Balance}, account {employerAccountId}, job id {jobId}");
            var fundingSourceEvents = new List<FundingSourcePaymentEvent>();
            fundingSourceEvents.AddRange(orderedRequiredLevyPayments.SelectMany(payment =>
                fundingSourcePaymentEventBuilder.BuildFundingSourcePaymentsForRequiredPayment(payment, employerAccountId, jobId)));

            logger.LogDebug($"Created {fundingSourceEvents.Count} payments - {GetFundsDebugString(fundingSourceEvents)}, account {employerAccountId}, job id {jobId}");

            levyAccount.Balance = levyBalanceService.RemainingBalance;
            levyAccount.TransferAllowance = levyBalanceService.RemainingTransferAllowance;
            await levyAccountCache.AddOrReplace(CacheKeys.LevyBalanceKey, levyAccount);

            logger.LogInfo($"Finished generating levy and/or co-invested payments for the account: {employerAccountId}, number of payments: {fundingSourceEvents.Count}.");
            return fundingSourceEvents.AsReadOnly();
        }

        private async Task<List<CalculatedRequiredLevyAmount>> GetOrderedCalculatedRequiredLevyAmounts(long employerAccountId)
        {
            var priorities = await dataContext.GetEmployerProviderPriorities(employerAccountId, CancellationToken.None);
            var prioritiesTuple =    priorities.Select(p => Tuple.Create(p.Ukprn, p.Order).ToValueTuple()).ToList();

            var transactions = await dataContext
                .GetTransactionsToBePaidByEmployer(employerAccountId);

            var calculatedRequiredLevyAmounts = transactions.Select(pt =>
                    JsonConvert.DeserializeObject<CalculatedRequiredLevyAmount>(pt.MessagePayload))
                .ToList();

           return await calculatedRequiredLevyAmountPrioritisationService.Prioritise(calculatedRequiredLevyAmounts, prioritiesTuple);
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
    }
} 