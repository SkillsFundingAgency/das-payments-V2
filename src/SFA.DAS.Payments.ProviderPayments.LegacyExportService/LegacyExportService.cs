using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.ProviderPayments.LegacyExportService
{
    [StatePersistence(StatePersistence.Persisted)]
    public class LegacyPaymentsExportService : StatefulService
    {
        private IStatefulEndpointCommunicationListener listener;
        private readonly ILifetimeScope lifetimeScope;
        private readonly IPaymentLogger paymentLogger;
        
        public LegacyPaymentsExportService(StatefulServiceContext context, ILifetimeScope lifetimeScope, IPaymentLogger paymentLogger)
            : base(context)
        {
            this.lifetimeScope = lifetimeScope;
            this.paymentLogger = paymentLogger;
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            paymentLogger.LogInfo("Creating Service Replica Listeners For LegacyExportService");
            return new List<ServiceReplicaListener>
            {
                new ServiceReplicaListener(context => listener = lifetimeScope.Resolve<IStatefulEndpointCommunicationListener>())
            };
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.WhenAll(listener.RunAsync());
        }
    }
}
