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
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers
{
    public class EarningEventHandler : IHandleMessages<EarningEvent>
    {
        private readonly IApprenticeshipKeyService apprenticeshipKeyService;
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger paymentLogger;
        private readonly ESFA.DC.Logging.ExecutionContext executionContext;

        public EarningEventHandler(IApprenticeshipKeyService apprenticeshipKeyService,
            IActorProxyFactory proxyFactory,
            IPaymentLogger paymentLogger,
            IExecutionContext executionContext)
        {
            this.apprenticeshipKeyService = apprenticeshipKeyService;
            this.proxyFactory = proxyFactory ?? new ActorProxyFactory();
            this.paymentLogger = paymentLogger;
            this.executionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
        }

        public async Task Handle(EarningEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing LevyFundedProxyService event. Message Id : {context.MessageId}");
            executionContext.JobId = message.JobId.ToString();

            try
            {
                var key = apprenticeshipKeyService.GenerateApprenticeshipKey(
                    message.Ukprn,
                    message.Learner.ReferenceNumber,
                    message.LearningAim.FrameworkCode,
                    message.LearningAim.PathwayCode,
                    message.LearningAim.ProgrammeType,
                    message.LearningAim.StandardCode,
                    message.LearningAim.Reference
                );

                var actorId = new ActorId(key);
                var actor = proxyFactory.CreateActorProxy<ILevyFundedService>(new Uri("fabric:/SFA.DAS.Payments.FundingSource.ServiceFabric/LevyFundedServiceActorService"), actorId);
                IReadOnlyCollection<RequiredPaymentEvent> requiredPaymentEvent;
                try
                {
                    requiredPaymentEvent = await actor.HandleEarningEvent(message, CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    paymentLogger.LogError($"Error invoking required payments actor. Error: {ex.Message}", ex);
                    throw;
                }

                try
                {
                    if (requiredPaymentEvent != null)
                        await Task.WhenAll(requiredPaymentEvent.Select(context.Publish)).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    //TODO: add more details when we flesh out the event.
                    paymentLogger.LogError($"Error publishing the event: 'RequiredPaymentEvent'.  Error: {ex.Message}.", ex);
                    throw;
                    //TODO: update the job
                }

                paymentLogger.LogInfo($"Successfully processed RequiredPaymentsProxyService event for Actor Id {actorId}");
            }
            catch (Exception ex)
            {
                paymentLogger.LogError("Error while handling RequiredPaymentsProxyService event", ex);
                throw;
            }
        }
    }
}