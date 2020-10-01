﻿using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors.Client;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService.Handlers
{
    public class Act1RedundancyFunctionalSkillEarningsEventHandler: EarningEventHandlerBase<Act1RedundancyFunctionalSkillEarningsEvent>
    {
        public Act1RedundancyFunctionalSkillEarningsEventHandler(IApprenticeshipKeyService apprenticeshipKeyService, IActorProxyFactory proxyFactory, IPaymentLogger paymentLogger, IExecutionContext executionContext) : base(apprenticeshipKeyService, proxyFactory, paymentLogger, executionContext)
        {
        }

        protected override async  Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandleEarningEvent(Act1RedundancyFunctionalSkillEarningsEvent message, IRequiredPaymentsService actor)
        {
            return await actor.HandleFunctionalSkillEarningsEvent(message, CancellationToken.None).ConfigureAwait(false);
        }
    }
}