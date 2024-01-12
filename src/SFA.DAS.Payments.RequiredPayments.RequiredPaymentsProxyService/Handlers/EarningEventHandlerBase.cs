using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Common.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService.Handlers
{
    public abstract class EarningEventHandlerBase<T> : IHandleMessages<T> where T : PaymentsEvent
    {
        private readonly IApprenticeshipKeyService apprenticeshipKeyService;
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger paymentLogger;
        private readonly ESFA.DC.Logging.ExecutionContext executionContext;

        protected EarningEventHandlerBase(IApprenticeshipKeyService apprenticeshipKeyService,
            IActorProxyFactory proxyFactory,
            IPaymentLogger paymentLogger,
            IExecutionContext executionContext)
        {
            this.apprenticeshipKeyService = apprenticeshipKeyService;
            this.proxyFactory = proxyFactory ?? new ActorProxyFactory();
            this.paymentLogger = paymentLogger;
            this.executionContext = (ESFA.DC.Logging.ExecutionContext) executionContext;
        }

        public async Task Handle(T message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing RequiredPaymentsProxyService event. Message Id : {context.MessageId}");
            executionContext.JobId = message.JobId.ToString();

            var contractType = GetContractTypeFromMessage(message);

            var key = apprenticeshipKeyService.GenerateApprenticeshipKey(
                message.Ukprn,
                message.Learner.ReferenceNumber,
                message.LearningAim.FrameworkCode,
                message.LearningAim.PathwayCode,
                message.LearningAim.ProgrammeType,
                message.LearningAim.StandardCode,
                message.LearningAim.Reference,
                message.CollectionPeriod.AcademicYear,
                contractType
            );

            var actorId = new ActorId(key);
            var actor = proxyFactory.CreateActorProxy<IRequiredPaymentsService>(
                new Uri("fabric:/SFA.DAS.Payments.RequiredPayments.ServiceFabric/RequiredPaymentsServiceActorService"),
                actorId);
            IReadOnlyCollection<PeriodisedRequiredPaymentEvent> requiredPaymentEvent;

            requiredPaymentEvent = await HandleEarningEvent(message, actor).ConfigureAwait(false);

            if (requiredPaymentEvent != null)
                await Task.WhenAll(requiredPaymentEvent.Select(context.Publish)).ConfigureAwait(false);

            paymentLogger.LogInfo("Successfully processed RequiredPaymentsProxyService event for Actor for " + 
            $"jobId:{message.JobId}, learnerRef:{message.Learner.ReferenceNumber}, frameworkCode:{message.LearningAim.FrameworkCode}, " +
            $"pathwayCode:{message.LearningAim.PathwayCode}, programmeType:{message.LearningAim.ProgrammeType}, " +
            $"standardCode:{message.LearningAim.StandardCode}, learningAimReference:{message.LearningAim.Reference}, " +
            $"academicYear:{message.CollectionPeriod.AcademicYear}, contractType:{contractType}");
        }

        private ContractType GetContractTypeFromMessage(T message)
        {
            if (message is PayableEarningEvent || message is ApprenticeshipContractType1RedundancyEarningEvent)
                return ContractType.Act1;

            if (message is ApprenticeshipContractType2EarningEvent || message is ApprenticeshipContractType2RedundancyEarningEvent)
                return ContractType.Act2;

            if (message is IFunctionalSkillEarningEvent funcSkill)
                return funcSkill.ContractType;

            throw new InvalidOperationException($"Cannot resolve contract type for {typeof(T).FullName}");
        }

        protected abstract Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandleEarningEvent(T message,
            IRequiredPaymentsService actor);
    }
}