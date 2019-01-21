using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers
{
    public class ApprenticeshipContractType1RequiredPaymentEventHandler : IHandleMessages<ApprenticeshipContractType1RequiredPaymentEvent>
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger paymentLogger;
        private readonly ESFA.DC.Logging.ExecutionContext executionContext;

        public ApprenticeshipContractType1RequiredPaymentEventHandler(IActorProxyFactory proxyFactory,
            IPaymentLogger paymentLogger,
            IExecutionContext executionContext)
        {
            this.proxyFactory = proxyFactory ?? new ActorProxyFactory();
            this.paymentLogger = paymentLogger;
            this.executionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
        }

        public async Task Handle(ApprenticeshipContractType1RequiredPaymentEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing LevyFundedProxyService event. Message Id : {context.MessageId}");
            executionContext.JobId = message.JobId.ToString();

            try
            {
                var actorId = new ActorId(message.EmployerAccountId);
                var actor = proxyFactory.CreateActorProxy<ILevyFundedService>(new Uri("fabric:/SFA.DAS.Payments.FundingSource.ServiceFabric/LevyFundedServiceActorService"), actorId);
                IReadOnlyCollection<FundingSourcePaymentEvent> fundingSourcePaymentEvents;
                try
                {
                    fundingSourcePaymentEvents = await actor.HandleRequiredPayment(message, CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    paymentLogger.LogError($"Error invoking levy funded actor. Error: {ex.Message}", ex);
                    throw;
                }

                try
                {
                    if (fundingSourcePaymentEvents != null)
                        await Task.WhenAll(fundingSourcePaymentEvents.Select(context.Publish)).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    //TODO: add more details when we flesh out the event.
                    paymentLogger.LogError($"Error publishing the event: 'RequiredPaymentEvent'.  Error: {ex.Message}.", ex);
                    throw;
                    //TODO: update the job
                }

                paymentLogger.LogInfo($"Successfully processed LevyFundedProxyService event for Actor Id {actorId}");
            }
            catch (Exception ex)
            {
                paymentLogger.LogError("Error while handling LevyFundedProxyService event", ex);
                throw;
            }
        }
    }
}