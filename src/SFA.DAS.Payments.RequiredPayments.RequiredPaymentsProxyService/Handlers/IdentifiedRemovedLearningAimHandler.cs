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
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;
using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService.Handlers
{
    public class IdentifiedRemovedLearningAimHandler : IHandleMessages<IdentifiedRemovedLearningAim>
    {
        private readonly IApprenticeshipKeyService apprenticeshipKeyService;
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger logger;
        private readonly IExecutionContext executionContext;

        public IdentifiedRemovedLearningAimHandler(IApprenticeshipKeyService apprenticeshipKeyService,  IActorProxyFactory proxyFactory, IPaymentLogger logger, IExecutionContext executionContext)
        {
            this.apprenticeshipKeyService = apprenticeshipKeyService;
            this.proxyFactory = proxyFactory;
            this.logger = logger;
            this.executionContext = executionContext;
        }

        public async Task Handle(IdentifiedRemovedLearningAim message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Processing 'IdentifiedRemovedLearningAim' message.");
            ((ExecutionContext)executionContext).JobId = message.JobId.ToString();

            var key = apprenticeshipKeyService.GenerateApprenticeshipKey(
                message.Ukprn,
                message.Learner.ReferenceNumber,
                message.LearningAim.FrameworkCode,
                message.LearningAim.PathwayCode,
                message.LearningAim.ProgrammeType,
                message.LearningAim.StandardCode,
                message.LearningAim.Reference,
                message.CollectionPeriod.AcademicYear,
                message.ContractType,false);

            var actorId = new ActorId(key);
            var actor = proxyFactory.CreateActorProxy<IRequiredPaymentsService>(new Uri("fabric:/SFA.DAS.Payments.RequiredPayments.ServiceFabric/RequiredPaymentsServiceActorService"), actorId);
            IReadOnlyCollection<PeriodisedRequiredPaymentEvent> requiredPayments = await actor.RefundRemovedLearningAim(message, CancellationToken.None).ConfigureAwait(false);
            logger.LogDebug($"Got {requiredPayments?.Count ?? 0} required payments.");
            if (requiredPayments != null)
                await Task.WhenAll(requiredPayments.Select(context.Publish)).ConfigureAwait(false);
            logger.LogInfo($"Successfully processed IdentifiedRemovedLearningAim event for Actor Id {actorId}");
        }
    }
}