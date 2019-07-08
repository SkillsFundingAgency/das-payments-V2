using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.BatchWriting;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.DataLocks.DataLockEventService
{
    [StatePersistence(StatePersistence.Persisted)]
    internal sealed class DataLockEventService : StatefulService
    {
        private IStatefulEndpointCommunicationListener listener;
        private readonly ILifetimeScope lifetimeScope;
        private readonly IPaymentLogger paymentLogger;
        private readonly IBatchProcessingService<DataLockStatusChanged> batchProcessingService;

        public DataLockEventService(StatefulServiceContext context, ILifetimeScope lifetimeScope, IPaymentLogger paymentLogger, IBatchProcessingService<DataLockStatusChanged> batchProcessingService)
            : base(context)
        {
            this.lifetimeScope = lifetimeScope;
            this.paymentLogger = paymentLogger;
            this.batchProcessingService = batchProcessingService;
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            paymentLogger.LogInfo("Creating Service Replica Listeners For DataLockEventService");
            return new List<ServiceReplicaListener>
            {
                new ServiceReplicaListener(context => listener = lifetimeScope.Resolve<IStatefulEndpointCommunicationListener>())
            };
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.WhenAll(listener.RunAsync(), batchProcessingService.RunAsync(cancellationToken));
        }
    }
}
