using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.Audit.EarningEventsService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    [StatePersistence(StatePersistence.Persisted)]
    public class EarningEventsService : AuditStatefulService<PaymentsDueEventModel>
    {
        public EarningEventsService(StatefulServiceContext context, IPaymentLogger logger, ILifetimeScope lifetimeScope, IPaymentsEventModelBatchService<EarningEventsEventModel> batchService) : base(context, logger, lifetimeScope, batchService)
        {
        }
    }
}
