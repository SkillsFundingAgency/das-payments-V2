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
    public class EarningEventsService : StatefulService
    {
        private readonly IPaymentLogger logger;
        private readonly ILifetimeScope lifetimeScope;
        private IStatefulEndpointCommunicationListener listener;
        public EarningEventsService(StatefulServiceContext context, IPaymentLogger logger, ILifetimeScope lifetimeScope)
            : base(context)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            logger.LogInfo("Creating Service Replica Listeners For Audit EarningEvents Service");

            return new List<ServiceReplicaListener>
            {
                new ServiceReplicaListener(context => listener = lifetimeScope.Resolve<IStatefulEndpointCommunicationListener>())
            };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return listener.RunAsync();

            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.
            
            //var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            //while (true)
            //{
            //    cancellationToken.ThrowIfCancellationRequested();

            //    using (var tx = this.StateManager.CreateTransaction())
            //    {
            //        var result = await myDictionary.TryGetValueAsync(tx, "Counter");

            //        ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
            //            result.HasValue ? result.Value.ToString() : "Value does not exist.");

            //        await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

            //        // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
            //        // discarded, and nothing is saved to the secondary replicas.
            //        await tx.CommitAsync();
            //    }

            //    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            //}
        }
    }
}
