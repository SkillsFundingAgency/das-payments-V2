using System.Fabric;
using Autofac;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Audit.Application.ServiceFabric.Infrastructure;
using SFA.DAS.Payments.Audit.Model;

namespace SFA.DAS.Payments.Audit.DatalockService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    [StatePersistence(StatePersistence.Persisted)]
    internal sealed class DatalockService : AuditStatefulService<DataLockEventModel>
    {
        public DatalockService(StatefulServiceContext context, IPaymentLogger logger, ILifetimeScope lifetimeScope, IPaymentsEventModelBatchService<DataLockEventModel> batchService) : base(context, logger, lifetimeScope, batchService)
        {
        }
    }
}
