using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors.Client;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService.Handlers
{
    public class ApprenticeshipContractType2EarningEventHandler : EarningEventHandlerBase<ApprenticeshipContractType2EarningEvent>
    {
        public ApprenticeshipContractType2EarningEventHandler(IApprenticeshipKeyService apprenticeshipKeyService, IActorProxyFactory proxyFactory, IPaymentLogger paymentLogger, IExecutionContext executionContext) 
            : base(apprenticeshipKeyService, proxyFactory, paymentLogger, executionContext)
        {
        }
    }
}