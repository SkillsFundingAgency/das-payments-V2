using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsProxyService.Handlers
{
    public class ProcessProviderMonthEndCommandHandler : IHandleMessages<ProcessProviderMonthEndCommand>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IExecutionContext executionContext;
        private readonly IActorProxyFactory proxyFactory;

        public ProcessProviderMonthEndCommandHandler(IPaymentLogger paymentLogger,
            IExecutionContext executionContext,
            IActorProxyFactory proxyFactory)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
            this.proxyFactory = proxyFactory;
        }


        public async Task Handle(ProcessProviderMonthEndCommand message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing Provider Month End Command for Message Id : {context.MessageId}");

            var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
            currentExecutionContext.JobId = message.JobId.ToString();

            try
            {
                var actorId = new ActorId(message.Ukprn.ToString());
                var actor = proxyFactory.CreateActorProxy<ProviderPaymentsService.Interfaces.IProviderPaymentsService>(new Uri("fabric:/SFA.DAS.Payments.ProviderPayments.ServiceFabric/ProviderPaymentsServiceActorService"), actorId);
                await actor.HandleMonthEnd(message.CollectionPeriod.Year, message.CollectionPeriod.Month, new CancellationToken());

                paymentLogger.LogInfo($"Successfully processed Month End Command for Job Id {message.JobId} and Message Type {message.GetType().Name}");
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Error while processing Process Provider Month End Command", ex);
                throw;
            }
        }
    }
}
