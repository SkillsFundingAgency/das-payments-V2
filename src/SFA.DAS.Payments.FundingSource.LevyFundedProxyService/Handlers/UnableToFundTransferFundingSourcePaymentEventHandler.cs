using System;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.FundingSource.Messages.Commands;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers
{
    public class UnableToFundTransferFundingSourcePaymentEventHandler: IHandleMessages<ProcessUnableToFundTransferFundingSourcePayment>
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger logger;
        private readonly IExecutionContext executionContext;

        public UnableToFundTransferFundingSourcePaymentEventHandler(IActorProxyFactory proxyFactory,
            IPaymentLogger logger, IExecutionContext executionContext)
        {
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
        }

        public async Task Handle(ProcessUnableToFundTransferFundingSourcePayment message, IMessageHandlerContext context)
        {
            logger.LogInfo($"Processing ProcessUnableToFundTransferFundingSourcePayment event. Message Id: {message.EventId}, Learner: {message.Learner?.ReferenceNumber}, Job: {message.JobId}, UKPRN: {message.Ukprn}");
            ((ESFA.DC.Logging.ExecutionContext)executionContext).JobId = message.JobId.ToString();

            if (!message.AccountId.HasValue)
                throw new ArgumentException($"Employer AccountId cannot be null. Event Id: {message.EventId}");

            logger.LogDebug($"Sending message to actor: {message.AccountId.Value}.");
            var actorId = new ActorId(message.AccountId.Value.ToString());
            var actor = proxyFactory.CreateActorProxy<ILevyFundedService>(new Uri("fabric:/SFA.DAS.Payments.FundingSource.ServiceFabric/LevyFundedServiceActorService"), actorId);
            var fundingSourceEvents = await actor.UnableToFundTransfer(message).ConfigureAwait(false);
            await Task.WhenAll(fundingSourceEvents.Select(context.Publish));
            logger.LogInfo($"Successfully processed ProcessUnableToFundTransferFundingSourcePayment event for Actor Id {actorId}, Learner: {message.Learner?.ReferenceNumber}, Job: {message.JobId}, UKPRN: {message.Ukprn}");
        }
    }
}