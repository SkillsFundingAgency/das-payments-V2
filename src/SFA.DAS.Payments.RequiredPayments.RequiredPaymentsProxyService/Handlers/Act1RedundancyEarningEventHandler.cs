using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService.Handlers
{
    public class Act1RedundancyEarningEventHandler : EarningEventHandlerBase<ApprenticeshipContractType1RedundancyEarningEvent>
    {
        public Act1RedundancyEarningEventHandler(IApprenticeshipKeyService apprenticeshipKeyService, IActorProxyFactory proxyFactory, IPaymentLogger paymentLogger, IExecutionContext executionContext) : base(apprenticeshipKeyService, proxyFactory, paymentLogger, executionContext)
        {
        }

        protected override async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandleEarningEvent(ApprenticeshipContractType1RedundancyEarningEvent message, IRequiredPaymentsService actor)
        {
            return await actor.HandleAct1RedundancyEarningsEvent(message, CancellationToken.None).ConfigureAwait(false);

        }
    }
}