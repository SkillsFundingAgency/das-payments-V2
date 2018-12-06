using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Audit.Application.ServiceFabric.Infrastructure;
using SFA.DAS.Payments.Audit.Model;

namespace SFA.DAS.Payments.Audit.PaymentsDueService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public class PaymentsDueService : AuditStatefulService<PaymentsDueEventModel>
    {
        public PaymentsDueService(StatefulServiceContext context, IPaymentLogger logger, ILifetimeScope lifetimeScope, IPaymentsEventModelBatchService<PaymentsDueEventModel> batchService) : base(context, logger, lifetimeScope, batchService)
        {
        }
    }
}
