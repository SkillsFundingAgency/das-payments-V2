using AutoMapper;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Monitoring.Jobs.Client;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Messages;
using SFA.DAS.Payments.ProviderPayments.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class FundingSourcePaymentEventHandler : IHandleMessages<FundingSourcePaymentEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IProviderPaymentsService paymentsService;
        private readonly IMapper mapper;
        private readonly IMonthEndService monthEndService;
        private readonly IEarningsJobClient earningsJobClient;

        public FundingSourcePaymentEventHandler(IPaymentLogger paymentLogger,
         IProviderPaymentsService paymentsService,
            IMapper mapper,
            IMonthEndService monthEndService,
            IEarningsJobClient earningsJobClient)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.paymentsService = paymentsService ?? throw new ArgumentNullException(nameof(paymentsService));
            this.mapper = mapper;
            this.monthEndService = monthEndService;
            this.earningsJobClient = earningsJobClient;
        }

        public async Task Handle(FundingSourcePaymentEvent message, IMessageHandlerContext context)
        {
            try
            {
                paymentLogger.LogDebug($"Processing Funding Source Payment Event for Message Id : {context.MessageId}");
                var paymentModel = mapper.Map<ProviderPaymentEventModel>(message);
                await paymentsService.ProcessPayment(paymentModel, default(CancellationToken));

                var isMonthEnd = await monthEndService
                    .MonthEndStarted(message.Ukprn, message.CollectionPeriod.AcademicYear, message.CollectionPeriod.Period)
                    .ConfigureAwait(false);

                if (!isMonthEnd)
                    return;

                paymentLogger.LogVerbose($"Processing Month End for {paymentModel.GetType().Name} Ukprn {message.Ukprn} - AcademicYear {message.CollectionPeriod.AcademicYear} - Period {message.CollectionPeriod.Period}");

                var monthEndJobId = await monthEndService.GetMonthEndJobId(message.Ukprn,
                    message.CollectionPeriod.AcademicYear,
                    message.CollectionPeriod.Period).ConfigureAwait(false);

                var paymentEvent = MapToProviderPaymentEvent(message, monthEndJobId, paymentModel.EventId);

                await context.Publish(paymentEvent);

                await earningsJobClient.ProcessedJobMessage(monthEndJobId,
                    paymentEvent.EventId,
                    paymentEvent.GetType().FullName,
                    new List<GeneratedMessage>());
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Error handling payment. Ukprn: {message.Ukprn}, JobId: {message.JobId}, Learner: {message.Learner.ReferenceNumber}, ContractType: {message.ContractType:G}, Transaction Type: {message.TransactionType:G}.  Error: {ex}", ex);
                throw;
            }


            paymentLogger.LogDebug($"finished processing Funding Source Payment Event for Message Id : {context.MessageId}.  {message.ToDebug()}");
        }

        private ProviderPaymentEvent MapToProviderPaymentEvent(FundingSourcePaymentEvent fundingSourcePaymentEvent, long monthEndJobId, Guid eventId)
        {
            paymentLogger.LogVerbose($"Mapping funding source payment: {fundingSourcePaymentEvent.ToDebug()}, funding source: {fundingSourcePaymentEvent.FundingSourceType:G}");
            var providerPayment = mapper.Map<ProviderPaymentEvent>(fundingSourcePaymentEvent);
            providerPayment.JobId = monthEndJobId;
            providerPayment.EventId = eventId;
            paymentLogger.LogDebug($"Finished mapping payment. Id: {providerPayment.EventId}");
            return providerPayment;
        }
    }
}
