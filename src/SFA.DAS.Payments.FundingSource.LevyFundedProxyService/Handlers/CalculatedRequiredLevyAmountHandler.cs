using System;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers
{
    public class CalculatedRequiredLevyAmountHandler : IHandleMessages<CalculatedRequiredLevyAmount>
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger paymentLogger;
        private readonly ILevyMessageRoutingService levyMessageRoutingService;
        private readonly ESFA.DC.Logging.ExecutionContext executionContext;

        public CalculatedRequiredLevyAmountHandler(IActorProxyFactory proxyFactory,
            IPaymentLogger paymentLogger,
            IExecutionContext executionContext,
            ILevyMessageRoutingService levyMessageRoutingService)
        {
            this.proxyFactory = proxyFactory ?? new ActorProxyFactory();
            this.paymentLogger = paymentLogger;
            this.levyMessageRoutingService = levyMessageRoutingService ?? throw new ArgumentNullException(nameof(levyMessageRoutingService));
            this.executionContext = (ESFA.DC.Logging.ExecutionContext) executionContext;
        }

        public async Task Handle(CalculatedRequiredLevyAmount message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing ApprenticeshipContractType1RequiredPaymentEvent event. Message Id: {context.MessageId}, Job: {message.JobId}");
            executionContext.JobId = message.JobId.ToString();

            if(!message.AccountId.HasValue)
               throw new ArgumentException($"Employer AccountId cannot be null. Event id:  {message.EventId}");

            var accountToUse = levyMessageRoutingService.GetDestinationAccountId(message);
            paymentLogger.LogDebug($"Sending levy message to levy actor: {accountToUse}.  Account: {message.AccountId}, sender: {message.TransferSenderAccountId}. ");
            var actorId = new ActorId(accountToUse.ToString());
            var actor = proxyFactory.CreateActorProxy<ILevyFundedService>(new Uri("fabric:/SFA.DAS.Payments.FundingSource.ServiceFabric/LevyFundedServiceActorService"), actorId);
            await actor.HandleRequiredPayment(message).ConfigureAwait(false);
            paymentLogger.LogInfo($"Successfully processed LevyFundedProxyService event for Actor Id {actorId}, Job: {message.JobId}");
        }
    }
}