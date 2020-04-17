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
using SFA.DAS.Payments.Messages.Core.Events;
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
            var isRedundancyPayment = IsPaymentTypeOfRedundancy(message);

            var key = apprenticeshipKeyService.GenerateApprenticeshipKey(
                message.Ukprn,
                message.Learner.ReferenceNumber,
                message.LearningAim.FrameworkCode,
                message.LearningAim.PathwayCode,
                message.LearningAim.ProgrammeType,
                message.LearningAim.StandardCode,
                message.LearningAim.Reference,
                message.CollectionPeriod.AcademicYear,
                contractType,
                isRedundancyPayment
            );

            var actorId = new ActorId(key);
            var actor = proxyFactory.CreateActorProxy<IRequiredPaymentsService>(
                new Uri("fabric:/SFA.DAS.Payments.RequiredPayments.ServiceFabric/RequiredPaymentsServiceActorService"),
                actorId);
            IReadOnlyCollection<PeriodisedRequiredPaymentEvent> requiredPaymentEvent;

            requiredPaymentEvent = await HandleEarningEvent(message, actor).ConfigureAwait(false);

            if (requiredPaymentEvent != null)
                await Task.WhenAll(requiredPaymentEvent.Select(context.Publish)).ConfigureAwait(false);

            paymentLogger.LogInfo($"Successfully processed RequiredPaymentsProxyService event for Actor Id {actorId}");
        }

        private bool IsPaymentTypeOfRedundancy(T message)
        {
            return typeof(IRedundancyEarningEvent).IsAssignableFrom(typeof(T));
        }

        private ContractType GetContractTypeFromMessage(T message)
        {
            switch (message)
            {
                case PayableEarningEvent payableEarningEvent:
                case ApprenticeshipContractType1RedundancyEarningEvent contractType1RedundancyEarning:
                    return ContractType.Act1;
                case ApprenticeshipContractType2EarningEvent apprenticeshipContractType2Earning:
                case ApprenticeshipContractType2RedundancyEarningEvent apprenticeshipContractType2RedundancyEarning:
                    return ContractType.Act2;
                case IFunctionalSkillEarningEvent funcSkill:
                    return funcSkill.ContractType;
                default:
                    throw new InvalidOperationException($"Cannot resolve contract type for {typeof(T).FullName}");
            }
        }

        protected abstract Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandleEarningEvent(T message,
            IRequiredPaymentsService actor);
    }
}