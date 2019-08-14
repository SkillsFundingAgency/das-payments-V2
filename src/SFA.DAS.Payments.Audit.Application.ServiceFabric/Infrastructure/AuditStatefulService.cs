using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.Audit.Application.ServiceFabric.Infrastructure
{
    public abstract class AuditStatefulService<TPaymentEventModel> : StatefulService where TPaymentEventModel : PaymentsEventModel
    {
        private readonly IPaymentLogger logger;
        private readonly ILifetimeScope lifetimeScope;
        private readonly IPaymentsEventModelBatchService<TPaymentEventModel> batchService;
        private IStatefulEndpointCommunicationListener listener;
        protected AuditStatefulService(StatefulServiceContext context, IPaymentLogger logger, ILifetimeScope lifetimeScope, IPaymentsEventModelBatchService<TPaymentEventModel> batchService)
            : base(context)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            this.batchService = batchService ?? throw new ArgumentNullException(nameof(batchService));
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            logger.LogInfo("Creating Service Replica Listeners For Audit EarningEvents Service");
            return new List<ServiceReplicaListener>
            {
                new ServiceReplicaListener(context => listener = lifetimeScope.Resolve<IStatefulEndpointCommunicationListener>())
            };
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.WhenAll(listener.RunAsync(), batchService.RunAsync(cancellationToken));
        }
    }
}