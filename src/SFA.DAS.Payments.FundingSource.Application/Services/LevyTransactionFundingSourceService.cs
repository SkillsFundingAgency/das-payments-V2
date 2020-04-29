using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Application.Data;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public interface ILevyTransactionFundingSourceService
    {
        Task<ReadOnlyCollection<FundingSourcePaymentEvent>> HandleMonthEnd(long employerAccountId, long jobId);
    }
    
    public class LevyTransactionFundingSourceService : ILevyTransactionFundingSourceService
    {
        private readonly IPaymentLogger logger;
        private readonly IFundingSourceDataContext dataContext;
        private readonly ILevyBalanceService levyBalanceService;
        private readonly IMapper mapper;
        private readonly IPaymentProcessor processor;
        private readonly ILevyFundingSourceRepository levyFundingSourceRepository;

        public LevyTransactionFundingSourceService(IPaymentLogger logger, IFundingSourceDataContext dataContext, ILevyBalanceService levyBalanceService, IMapper mapper, IPaymentProcessor processor, ILevyFundingSourceRepository levyFundingSourceRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.levyBalanceService = levyBalanceService ?? throw new ArgumentNullException(nameof(levyBalanceService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
            this.levyFundingSourceRepository = levyFundingSourceRepository ?? throw new ArgumentNullException(nameof(levyFundingSourceRepository));
        }

        public async Task<ReadOnlyCollection<FundingSourcePaymentEvent>> HandleMonthEnd(long employerAccountId, long jobId)
        {
            var levyAccount = await levyFundingSourceRepository.GetLevyAccount(employerAccountId);
            levyBalanceService.Initialise(levyAccount.Balance, levyAccount.TransferAllowance);

            var requiredLevyPayments = await GetCalculatedRequiredLevyAmounts(employerAccountId).ConfigureAwait(false);
           
            //todo: ordering logic
            
            logger.LogDebug($"Processing {requiredLevyPayments.Count} required payments, levy balance {levyAccount.Balance}, account {employerAccountId}, job id {jobId}");
            var fundingSourceEvents = new List<FundingSourcePaymentEvent>();
            fundingSourceEvents.AddRange(requiredLevyPayments.SelectMany(payment =>
                CreateFundingSourcePaymentsForRequiredPayment(payment, employerAccountId, jobId)));

            logger.LogDebug($"Created {fundingSourceEvents.Count} payments - {GetFundsDebugString(fundingSourceEvents)}, account {employerAccountId}, job id {jobId}");

            logger.LogInfo($"Finished generating levy and/or co-invested payments for the account: {employerAccountId}, number of payments: {fundingSourceEvents.Count}.");
            return fundingSourceEvents.AsReadOnly();
        }

        private async Task<List<CalculatedRequiredLevyAmount>> GetCalculatedRequiredLevyAmounts(long employerAccountId)
        {
            var transactions = await dataContext.GetEmployerLevyTransactions(employerAccountId).ConfigureAwait(false);
            return transactions.Select(transaction => JsonConvert.DeserializeObject<CalculatedRequiredLevyAmount>(transaction.MessagePayload))
                .ToList();
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