using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors.Client;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Messages;
using SFA.DAS.Payments.RequiredPayments.Domain;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService.Handlers
{
    public class PayableEarningEventHandler : EarningEventHandlerBase<PayableEarningEvent>
    {
        public PayableEarningEventHandler(IApprenticeshipKeyService apprenticeshipKeyService, IActorProxyFactory proxyFactory, IPaymentLogger paymentLogger, IExecutionContext executionContext) 
            : base(apprenticeshipKeyService, proxyFactory, paymentLogger, executionContext)
        {
        }
    }
}