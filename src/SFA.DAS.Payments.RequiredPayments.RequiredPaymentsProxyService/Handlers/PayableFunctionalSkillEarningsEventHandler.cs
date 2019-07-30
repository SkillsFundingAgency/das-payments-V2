using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors.Client;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService.Handlers
{
    public class PayableFunctionalSkillEarningsEventHandler : EarningEventHandlerBase<PayableFunctionalSkillEarningEvent>
    {
        public PayableFunctionalSkillEarningsEventHandler(IApprenticeshipKeyService apprenticeshipKeyService, IActorProxyFactory proxyFactory, IPaymentLogger paymentLogger, IExecutionContext executionContext)
            : base(apprenticeshipKeyService, proxyFactory, paymentLogger, executionContext)
        {
        }

        protected override async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandleEarningEvent(PayableFunctionalSkillEarningEvent message, IRequiredPaymentsService actor)
        {
            return await actor.HandlePayableFunctionalSkillEarningsEvent(message, CancellationToken.None).ConfigureAwait(false);
        }
    }
}