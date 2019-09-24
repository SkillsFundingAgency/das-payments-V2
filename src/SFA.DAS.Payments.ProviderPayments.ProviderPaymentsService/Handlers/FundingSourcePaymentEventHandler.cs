using AutoMapper;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Monitoring.Jobs.Client;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class FundingSourcePaymentEventHandler : IHandleMessages<FundingSourcePaymentEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IProviderPaymentsService paymentsService;
        private readonly IMapper mapper;
        private readonly IJobMessageClient jobClient;
        private readonly IProcessAfterMonthEndPaymentService afterMonthEndPaymentService;

        public FundingSourcePaymentEventHandler(IPaymentLogger paymentLogger,
         IProviderPaymentsService paymentsService,
            IMapper mapper,
            IProviderPeriodEndService providerPeriodEndService,
            IEarningsJobClient earningsJobClient,
            IJobMessageClient jobClient,
            IProcessAfterMonthEndPaymentService afterMonthEndPaymentService)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.paymentsService = paymentsService ?? throw new ArgumentNullException(nameof(paymentsService));
            this.mapper = mapper;
            this.jobClient = jobClient;
            this.afterMonthEndPaymentService = afterMonthEndPaymentService;
        }

        public async Task Handle(FundingSourcePaymentEvent message, IMessageHandlerContext context)
        {
            try
            {
                paymentLogger.LogDebug($"Processing Funding Source Payment Event for Message Id : {context.MessageId}");
                var paymentModel = mapper.Map<ProviderPaymentEventModel>(message);
                await paymentsService.ProcessPayment(paymentModel, default(CancellationToken)).ConfigureAwait(false);

                var afterMonthEndPayment = await afterMonthEndPaymentService.GetPaymentEvent(message);
                if (afterMonthEndPayment != null)
                {
                    paymentLogger.LogInfo($"Sent {afterMonthEndPayment.GetType().Name} for {message.JobId} and Message Type {message.GetType().Name}");
                    paymentLogger.LogDebug($"Sending Provider Payment Event {JsonConvert.SerializeObject(afterMonthEndPayment)} for Message Id : {context.MessageId}.  {message.ToDebug()}");
                    await context.Publish(afterMonthEndPayment).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Error handling payment. Ukprn: {message.Ukprn}, JobId: {message.JobId}, Learner: {message.Learner.ReferenceNumber}, ContractType: {message.ContractType:G}, Transaction Type: {message.TransactionType:G}.  Error: {ex}", ex);
                throw;
            }
            paymentLogger.LogDebug($"Finished processing Funding Source Payment Event for Message Id : {context.MessageId}.  {message.ToDebug()}");
        }
    }
}
