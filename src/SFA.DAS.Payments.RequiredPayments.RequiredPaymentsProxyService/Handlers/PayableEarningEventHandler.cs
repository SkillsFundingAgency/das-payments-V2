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
using SFA.DAS.Payments.ServiceFabric.Core;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;


namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService.Handlers
{
    public class PayableEarningEventHandler : IHandleMessages<PayableEarningEvent>
    {
        private readonly IApprenticeshipKeyService _apprenticeshipKeyService;
        private readonly IEndpointCommunicationSender<IPaymentsDueEvent> _endpoint;
        private readonly IActorProxyFactory _proxyFactory;
        private readonly IPaymentLogger _paymentLogger;
        private readonly ILifetimeScope _lifetimeScope;

        public PayableEarningEventHandler(IApprenticeshipKeyService apprenticeshipKeyService,
                                        IEndpointCommunicationSender<IPaymentsDueEvent> endpoint,
                                        IActorProxyFactory proxyFactory,
                                        IPaymentLogger paymentLogger,
                                        ILifetimeScope lifetimeScope)
        {
            _apprenticeshipKeyService = apprenticeshipKeyService;
            _endpoint = endpoint;
            _proxyFactory = proxyFactory ?? new ActorProxyFactory();
            _paymentLogger = paymentLogger;
            _lifetimeScope = lifetimeScope;
        }

        public async Task Handle(PayableEarningEvent message, IMessageHandlerContext context)
        {
            using (var scope = _lifetimeScope.BeginLifetimeScope())
            {
                _paymentLogger.LogInfo($"Processing RequiredPaymentsProxyService event. Message Id : {context.MessageId}", null, "", "", 0);

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
                    var paymentsDue = await actor.HandlePayableEarning(message, CancellationToken.None).ConfigureAwait(false);

                    //await Task.WhenAll(paymentsDue.Select(p => _endpoint.Send(p)).ToArray()).ConfigureAwait(false);

                    _paymentLogger.LogInfo($"Sucessfully processed RequiredPaymentsProxyService event for Actor Id {actorId}");
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