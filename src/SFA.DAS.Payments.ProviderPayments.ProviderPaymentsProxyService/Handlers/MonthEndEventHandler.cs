using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsProxyService.Handlers
{
    public class MonthEndEventHandler : IHandleMessages<MonthEndEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IExecutionContext executionContext;
        private readonly IActorProxyFactory proxyFactory;
        private readonly IMapper mapper;
        private readonly Application.Services.IMonthEndEventHandlerService monthEndEventHandlerService;

        public MonthEndEventHandler(IPaymentLogger paymentLogger,
            IExecutionContext executionContext,
            IActorProxyFactory proxyFactory,
            IMapper mapper,
            Application.Services.IMonthEndEventHandlerService  monthEndEventHandlerService)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
            this.proxyFactory = proxyFactory;
            this.mapper = mapper;
            this.monthEndEventHandlerService = monthEndEventHandlerService;
        }

        public async Task Handle(MonthEndEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing Month End Event for Message Id : {context.MessageId}");

            var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
            currentExecutionContext.JobId = message.JobId.ToString();

            try
            {
                var actorId = new ActorId(message.Ukprn.ToString());
                var actor = proxyFactory.CreateActorProxy<ProviderPaymentsService.Interfaces.IProviderPaymentsService>(new Uri("fabric:/SFA.DAS.Payments.ProviderPayments.ServiceFabric/ProviderPaymentsServiceActorService"), actorId);

                var paymentModel = mapper.Map<ProviderPeriodicPayment>(message);

                await actor.ha(paymentModel, new CancellationToken());


                paymentLogger.LogInfo($"Successfully processed Month End Event for Job Id {message.JobId} and Message Type {message.GetType().Name}");
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Error while handling Provider Payments ProxyService Month End Event ", ex);
                throw;
            }

            await Task.CompletedTask;
        }
    }
}
