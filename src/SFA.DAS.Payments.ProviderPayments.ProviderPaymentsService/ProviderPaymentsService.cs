using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using System.Fabric;
using Autofac;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Audit.Application.ServiceFabric.Infrastructure;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService
{

    [StatePersistence(StatePersistence.Persisted)]
    public class ProviderPaymentsStatefulService : AuditStatefulService<ProviderPaymentEventModel>
    {
        public ProviderPaymentsStatefulService(StatefulServiceContext context, IPaymentLogger logger, ILifetimeScope lifetimeScope, IPaymentsEventModelBatchService<ProviderPaymentEventModel> batchService) : base(context, logger, lifetimeScope, batchService)
        {
        }
    }
}
