using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers
{
    public class ReceivedProviderEarningsEventHandler : IHandleMessages<ReceivedProviderEarningsEvent>
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger logger;
        private readonly IProviderRepository providerRepository;

        public ReceivedProviderEarningsEventHandler(IActorProxyFactory proxyFactory, IPaymentLogger logger, IProviderRepository providerRepository)
        {
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.providerRepository = providerRepository ?? throw new ArgumentNullException(nameof(providerRepository));
        }

        public async Task Handle(ReceivedProviderEarningsEvent message, IMessageHandlerContext context)
        {
            try
            {
                logger.LogDebug($"Getting employer accounts linked to provider {message.Ukprn}.");
                var accountIds = await providerRepository.GetApprenticeshipEmployers(message.Ukprn);
                logger.LogDebug($"Got {accountIds.Count} employer accounts linked to provider: {message.Ukprn}");
                foreach (var accountId in accountIds)
                {
                    var actorId = new ActorId(accountId);
                    var actor = proxyFactory.CreateActorProxy<ILevyFundedService>(new Uri("fabric:/SFA.DAS.Payments.FundingSource.ServiceFabric/LevyFundedServiceActorService"), actorId);
                    await actor.HandleReceivedProviderEarnings(message).ConfigureAwait(false);
                    logger.LogInfo($"Successfully processed ReceivedProviderEarnings event for Actor Id {actorId}, Job: {message.JobId}, UKPRN: {message.Ukprn}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error handling ReceivedProviderEarningsEvent. Failed to notify all Levy Funding Source Actors of the new submission for Ukprn: {message.Ukprn}. Error: {ex.Message}");
                throw;
            }
        }
    }
}