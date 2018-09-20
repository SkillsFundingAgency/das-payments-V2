using Autofac;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.RequiredPayments.Domain.Enums;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain;


namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService.Handlers
{
    public class Act2PaymentDueEventHandler : IHandleMessages<ApprenticeshipContractType2PaymentDueEvent>
    {
        private readonly IApprenticeshipKeyService _apprenticeshipKeyService;
        private readonly IActorProxyFactory _proxyFactory;
        private readonly IPaymentLogger _paymentLogger;
        private readonly ILifetimeScope _lifetimeScope;

        public Act2PaymentDueEventHandler(IApprenticeshipKeyService apprenticeshipKeyService,
                                        IActorProxyFactory proxyFactory,
                                        IPaymentLogger paymentLogger,
                                        ILifetimeScope lifetimeScope)
        {
            _apprenticeshipKeyService = apprenticeshipKeyService;
            _proxyFactory = proxyFactory ?? new ActorProxyFactory();
            _paymentLogger = paymentLogger;
            _lifetimeScope = lifetimeScope;
        }

        public async Task Handle(ApprenticeshipContractType2PaymentDueEvent message, IMessageHandlerContext context)
        {
            using (_lifetimeScope.BeginLifetimeScope())
            {
                _paymentLogger.LogInfo($"Processing RequiredPaymentsProxyService event. Message Id : {context.MessageId}");

                var executionContext = (ESFA.DC.Logging.ExecutionContext) _lifetimeScope.Resolve<IExecutionContext>();
                executionContext.JobId = message.JobId;

                try
                {
                    var key = _apprenticeshipKeyService.GenerateApprenticeshipKey(
                        message.Ukprn,
                        message.Learner.ReferenceNumber,
                        message.LearningAim.FrameworkCode,
                        message.LearningAim.PathwayCode,
                        (ProgrammeType) message.LearningAim.ProgrammeType,
                        message.LearningAim.StandardCode,
                        message.LearningAim.Reference
                    );

                    var actorId = new ActorId(key);
                    var actor = _proxyFactory.CreateActorProxy<IRequiredPaymentsService>(new Uri("fabric:/SFA.DAS.Payments.RequiredPayments.ServiceFabric/RequiredPaymentsServiceActorService"), actorId);
                    ApprenticeshipContractType2RequiredPaymentEvent requiredPaymentEvent;
                    try
                    {
                        requiredPaymentEvent = await actor.HandleAct2PaymentDueEvent(message, CancellationToken.None).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _paymentLogger.LogError($"Error invoking required payments actor. Error: {ex.Message}", ex);
                        throw;
                    }

                    try
                    {
                        if (requiredPaymentEvent != null)
                            await context.Publish(requiredPaymentEvent).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        //TODO: add more details when we flesh out the event.
                        _paymentLogger.LogError($"Error publishing the event: 'RequiredPaymentEvent'.  Error: {ex.Message}.", ex);
                        throw;
                        //TODO: update the job
                    }

                    _paymentLogger.LogInfo($"Successfully processed RequiredPaymentsProxyService event for Actor Id {actorId}");
                }
                catch (Exception ex)
                {
                    _paymentLogger.LogError("Error while handling RequiredPaymentsProxyService event", ex);
                    throw;
                }
            }
        }
    }
}