using Autofac;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain.Enums;
using SFA.DAS.Payments.RequiredPayments.Domain.Interfaces;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;


namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService.Handlers
{
    public class PayableEarningEventHandler : IHandleMessages<PayableEarningEvent>
    {
        private readonly IApprenticeshipKeyService _apprenticeshipKeyService;
        private readonly IActorProxyFactory _proxyFactory;
        private readonly IPaymentLogger _paymentLogger;
        private readonly ILifetimeScope _lifetimeScope;

        public PayableEarningEventHandler(IApprenticeshipKeyService apprenticeshipKeyService,
                                        IActorProxyFactory proxyFactory,
                                        IPaymentLogger paymentLogger,
                                        ILifetimeScope lifetimeScope)
        {
            _apprenticeshipKeyService = apprenticeshipKeyService;
            _proxyFactory = proxyFactory ?? new ActorProxyFactory();
            _paymentLogger = paymentLogger;
            _lifetimeScope = lifetimeScope;
        }

        public async Task Handle(PayableEarningEvent message, IMessageHandlerContext context)
        {
            using (var scope = _lifetimeScope.BeginLifetimeScope())
            {
                _paymentLogger.LogInfo($"Processing RequiredPaymentsProxyService event. Message Id : {context.MessageId}");

                var executionContext = (ESFA.DC.Logging.ExecutionContext)_lifetimeScope.Resolve<IExecutionContext>();
                executionContext.JobId = message.JobId;

                try
                {
                    var key = _apprenticeshipKeyService.GenerateKey(
                        message.Ukprn,
                        message.LearnRefNumber,
                        message.LearnAim.FrameworkCode,
                        message.LearnAim.PathwayCode,
                        (ProgrammeType)message.LearnAim.ProgrammeType,
                        message.LearnAim.StandardCode,
                        message.LearnAim.LearnAimRef
                    );

                    var actorId = new ActorId(key);
                    var actor = _proxyFactory.CreateActorProxy<IRequiredPaymentsService>(new Uri("fabric:/SFA.DAS.Payments.RequiredPayments.ServiceFabric/RequiredPaymentsServiceActorService"), actorId);
                    CalculatedPaymentDueEvent[] paymentsDue;
                    try
                    {
                        paymentsDue = await actor.HandlePayableEarning(message, CancellationToken.None)
                            .ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _paymentLogger.LogError($"Error invoking required payments actor. Error: {ex.Message}", ex);
                        throw;
                    }

                    foreach (var calculatedPaymentDueEvent in paymentsDue)
                    {
                        try
                        {
                            await context.Publish(calculatedPaymentDueEvent);
                        }
                        catch (Exception ex)
                        {
                            //TODO: add more details when we flesh out the event.
                            _paymentLogger.LogError($"Error publishing the event: 'CalculatedPaymentDueEvent'.  Error: {ex.Message}.", ex);
                            throw;
                            //TODO: update the job
                        }
                    }
                    _paymentLogger.LogInfo($"Successfully processed RequiredPaymentsProxyService event for Actor Id {actorId}");
                }
                catch (Exception ex)
                {
                    _paymentLogger.LogError($"Error while handling RequiredPaymentsProxyService event", ex);
                    throw;
                }
            }
        }
    }
}