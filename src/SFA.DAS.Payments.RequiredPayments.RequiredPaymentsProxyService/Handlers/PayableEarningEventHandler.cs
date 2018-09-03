﻿using Autofac;
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
    public class PayableEarningEventHandler : IHandleMessages<IEarningEvent>
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

        public async Task Handle(IEarningEvent message, IMessageHandlerContext context)
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
                        message.Learner.ReferenceNumber,
                        message.LearningAim.FrameworkCode,
                        message.LearningAim.PathwayCode,
                        (ProgrammeType)message.LearningAim.ProgrammeType,
                        message.LearningAim.StandardCode,
                        message.LearningAim.Reference
                    );

                    var actorId = new ActorId(key);
                    var actor = _proxyFactory.CreateActorProxy<IRequiredPaymentsService>(new Uri("fabric:/SFA.DAS.Payments.RequiredPayments.ServiceFabric/RequiredPaymentsServiceActorService"), actorId);
                    RequiredPaymentEvent[] payments;
                    try
                    {
                        payments = await actor.HandleEarning(message, CancellationToken.None)
                            .ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _paymentLogger.LogError($"Error invoking required payments actor. Error: {ex.Message}", ex);
                        throw;
                    }

                    foreach (var calculatedPaymentDueEvent in payments)
                    {
                        try
                        {
                            await context.Publish(calculatedPaymentDueEvent);
                        }
                        catch (Exception ex)
                        {
                            //TODO: add more details when we flesh out the event.
                            _paymentLogger.LogError($"Error publishing the event: 'RequiredPaymentEvent'.  Error: {ex.Message}.", ex);
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