using System;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers
{
    public class UnableToFundTransferFundingSourcePaymentEventHandler: IHandleMessages<UnableToFundTransferFundingSourcePaymentEvent>
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

        public async Task Handle(UnableToFundTransferFundingSourcePaymentEvent message, IMessageHandlerContext context)
        {
            logger.LogInfo($"Processing UnableToFundTransferFundingSourcePaymentEvent event. Message Id: {message.EventId}, Learner: {message.Learner?.ReferenceNumber}, Job: {message.JobId}, UKPRN: {message.Ukprn}");
            ((ESFA.DC.Logging.ExecutionContext)executionContext).JobId = message.JobId.ToString();

            if (!message.AccountId.HasValue)
                throw new ArgumentException($"Employer AccountId cannot be null. Event Id: {message.EventId}");

            try
            {
                logger.LogDebug($"Sending message to actor: {message.AccountId.Value}.");
                var actorId = new ActorId(message.AccountId.Value);
                var actor = proxyFactory.CreateActorProxy<ILevyFundedService>(new Uri("fabric:/SFA.DAS.Payments.FundingSource.ServiceFabric/LevyFundedServiceActorService"), actorId);
                await actor.UnableToFundTransfer(message).ConfigureAwait(false);
                logger.LogInfo($"Successfully processed UnableToFundTransferFundingSourcePaymentEvent event for Actor Id {actorId}, Learner: {message.Learner?.ReferenceNumber}, Job: {message.JobId}, UKPRN: {message.Ukprn}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error while handling UnableToFundTransferFundingSourcePaymentEvent event. Error: {ex.Message}, Event Id: {message.EventId}, Learner: {message.Learner?.ReferenceNumber}, Job: {message.JobId}, UKPRN: {message.Ukprn}", ex);
                throw;
            }

        }
    }
}