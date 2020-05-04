using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Data;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
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
        private readonly IMapper mapper;
        private readonly IPaymentProcessor processor;
        private readonly ILevyFundingSourceRepository levyFundingSourceRepository;
        private readonly IDataCache<LevyAccountModel> levyAccountCache;
        private readonly ICalculatedRequiredLevyAmountPrioritisationService calculatedRequiredLevyAmountPrioritisationService;

        public FundingSourceEventGenerationService(IPaymentLogger logger, IFundingSourceDataContext dataContext, ILevyBalanceService levyBalanceService, IMapper mapper, IPaymentProcessor processor, ILevyFundingSourceRepository levyFundingSourceRepository, IDataCache<LevyAccountModel> levyAccountCache, ICalculatedRequiredLevyAmountPrioritisationService calculatedRequiredLevyAmountPrioritisationService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.levyBalanceService = levyBalanceService ?? throw new ArgumentNullException(nameof(levyBalanceService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
            this.levyFundingSourceRepository = levyFundingSourceRepository ?? throw new ArgumentNullException(nameof(levyFundingSourceRepository));
            this.levyAccountCache = levyAccountCache ?? throw new ArgumentNullException(nameof(levyAccountCache));
            this.calculatedRequiredLevyAmountPrioritisationService = calculatedRequiredLevyAmountPrioritisationService ?? throw new ArgumentNullException(nameof(calculatedRequiredLevyAmountPrioritisationService));
        }

        public async Task<ReadOnlyCollection<FundingSourcePaymentEvent>> HandleMonthEnd(long employerAccountId, long jobId)
        {
            var levyAccount = await levyFundingSourceRepository.GetLevyAccount(employerAccountId);
            levyBalanceService.Initialise(levyAccount.Balance, levyAccount.TransferAllowance);

            var orderedRequiredLevyPayments = await GetOrderedCalculatedRequiredLevyAmounts(employerAccountId).ConfigureAwait(false);

            logger.LogDebug($"Processing {orderedRequiredLevyPayments.Count} required payments, levy balance {levyAccount.Balance}, account {employerAccountId}, job id {jobId}");
            var fundingSourceEvents = new List<FundingSourcePaymentEvent>();
            fundingSourceEvents.AddRange(orderedRequiredLevyPayments.SelectMany(payment =>
                CreateFundingSourcePaymentsForRequiredPayment(payment, employerAccountId, jobId)));

            logger.LogDebug($"Created {fundingSourceEvents.Count} payments - {GetFundsDebugString(fundingSourceEvents)}, account {employerAccountId}, job id {jobId}");

            levyAccount.Balance = levyBalanceService.RemainingBalance;
            levyAccount.TransferAllowance = levyBalanceService.RemainingTransferAllowance;
            await levyAccountCache.AddOrReplace(CacheKeys.LevyBalanceKey, levyAccount);

            logger.LogInfo($"Finished generating levy and/or co-invested payments for the account: {employerAccountId}, number of payments: {fundingSourceEvents.Count}.");
            return fundingSourceEvents.AsReadOnly();
        }

        private async Task<List<CalculatedRequiredLevyAmount>> GetOrderedCalculatedRequiredLevyAmounts(long employerAccountId)
        {
            var priorities = dataContext.EmployerProviderPriorities.Where(x => x.EmployerAccountId == employerAccountId)
                .Select(p => Tuple.Create(p.Ukprn, p.Order).ToValueTuple()).ToList();

            var transactions = dataContext
                .GetEmployerLevyTransactions(employerAccountId).ToList();

            var calculatedRequiredLevyAmounts = transactions.Select(pt =>
                    JsonConvert.DeserializeObject<CalculatedRequiredLevyAmount>(pt.MessagePayload))
                .ToList();

           return await calculatedRequiredLevyAmountPrioritisationService.Prioritise(calculatedRequiredLevyAmounts,priorities);
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
    }
} 