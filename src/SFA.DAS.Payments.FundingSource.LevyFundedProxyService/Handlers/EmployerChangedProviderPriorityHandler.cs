using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers
{
    public class EmployerChangedProviderPriorityHandler : IHandleMessages<EmployerChangedProviderPriority>
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger paymentLogger;
        private readonly ESFA.DC.Logging.ExecutionContext executionContext;

        public EmployerChangedProviderPriorityHandler(IActorProxyFactory proxyFactory,
            IPaymentLogger paymentLogger,
            IExecutionContext executionContext)
        {
            this.proxyFactory = proxyFactory ?? new ActorProxyFactory();
            this.paymentLogger = paymentLogger;
            this.executionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
        }

        public async Task Handle(EmployerChangedProviderPriority message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing EmployerChangedProviderPriority event. Message Id: {context.MessageId}, Account Id: {message.EmployerAccountId}");
           
            try
            {
                var actorId = new ActorId(message.EmployerAccountId);
                var actor = proxyFactory.CreateActorProxy<ILevyFundedService>(new Uri("fabric:/SFA.DAS.Payments.FundingSource.ServiceFabric/LevyFundedServiceActorService"), actorId);
                await actor.HandleEmployerProviderPriorityChange(message).ConfigureAwait(false);

                paymentLogger.LogInfo($"Successfully processed EmployerChangedProviderPriority event for Actor Id {actorId} ,Account Id: {message.EmployerAccountId}");
            }
            catch (Exception ex)
            {
                paymentLogger.LogError($"Error while handling EmployerChangedProviderPriority event, Account Id: {message.EmployerAccountId}", ex);
                throw;
            }
        }
    }
}
