using System.Fabric;
using Autofac;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Audit.Application.ServiceFabric.Infrastructure;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.EarningEventsService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    [StatePersistence(StatePersistence.Persisted)]
    public class EarningEventsService : AuditStatefulService<EarningEventModel>
    {
        public EarningEventsService(StatefulServiceContext context, IPaymentLogger logger, ILifetimeScope lifetimeScope, IPaymentsEventModelBatchService<EarningEventModel> batchService) : base(context, logger, lifetimeScope, batchService)
        {
        }
    }
}
