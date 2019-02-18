using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ESFA.DC.Logging.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Jobs.Client;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Messages;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class ProcessProviderMonthEndCommandHandler : IHandleMessages<ProcessProviderMonthEndCommand>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IExecutionContext executionContext;
        private readonly IMonthEndService monthEndService;
        private readonly IMapper mapper;
        private readonly IProviderPaymentFactory paymentFactory;
        private readonly IEarningsJobClient jobClient;


        public ProcessProviderMonthEndCommandHandler(IPaymentLogger paymentLogger,
            IExecutionContext executionContext,
            IMonthEndService monthEndService,
            IMapper mapper,
            IProviderPaymentFactory paymentFactory, 
            IEarningsJobClient jobClient)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
            this.monthEndService = monthEndService ?? throw new ArgumentNullException(nameof(monthEndService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.paymentFactory = paymentFactory ?? throw new ArgumentNullException(nameof(paymentFactory));
            this.jobClient = jobClient ?? throw new ArgumentNullException(nameof(jobClient));
        }

        public async Task Handle(ProcessProviderMonthEndCommand message, IMessageHandlerContext context)
        {
            paymentLogger.LogDebug($"Processing Provider Month End Command. Ukprn: {message.Ukprn}, Academic Year:{message.CollectionPeriod.AcademicYear}, Collection Period: {message.CollectionPeriod.Period}.");
            var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
            currentExecutionContext.JobId = message.JobId.ToString();
            try
            {
                await monthEndService.StartMonthEnd(message.Ukprn, message.CollectionPeriod.AcademicYear, message.CollectionPeriod.Period, message.JobId);

                var payments = await monthEndService.GetMonthEndPayments(message.CollectionPeriod, message.Ukprn);
                foreach (var paymentEvent in payments.Select(payment => MapToProviderPaymentEvent(payment, message.JobId)))
                {
                    await context.Publish(paymentEvent);
                    await jobClient.ProcessedJobMessage(message.JobId, paymentEvent.EventId,
                        paymentEvent.GetType().FullName, new List<GeneratedMessage>());
                }
                paymentLogger.LogInfo($"Successfully processed Month End Command for Job Id {message.JobId} and Message Type {message.GetType().Name}");
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Error while processing Process Provider Month End Command. Error: {ex}", ex);
                throw;
            }
        }

        private ProviderPaymentEvent MapToProviderPaymentEvent(PaymentModel payment, long monthEndJobId)
        {
            paymentLogger.LogVerbose($"Mapping payment id: {payment.Id}, funding source: {payment.FundingSource}");
            var providerPayment = paymentFactory.Create(payment.FundingSource);
            paymentLogger.LogVerbose($"Got {providerPayment.GetType().Name} payment message type. Now mapping to provider payment.");
            mapper.Map(payment, providerPayment);
            providerPayment.JobId = monthEndJobId;
            paymentLogger.LogVerbose($"Finished mapping payment. Id: {providerPayment.EventId}");
            return providerPayment;
        }
    }
}
