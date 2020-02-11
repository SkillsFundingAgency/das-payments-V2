using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Query;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService
{
    public class LevyFundedProxyService : StatelessService
    {
        private IStatelessEndpointCommunicationListener listener;
        private readonly ILifetimeScope lifetimeScope;
        private readonly IPaymentLogger paymentLogger;

        public LevyFundedProxyService(StatelessServiceContext context, ILifetimeScope lifetimeScope, IPaymentLogger paymentLogger) : base(context)
        {
            this.lifetimeScope = lifetimeScope;
            this.paymentLogger = paymentLogger;
        }

        private async Task LogActorInformation()
        {
            using (var client = new FabricClient())
            {
                var applications = await client.QueryManager.GetApplicationListAsync();

                foreach (var application in applications)
                {
                    paymentLogger.LogInfo($"APPLICATION - {application.ApplicationTypeName}");
                    var services = await client.QueryManager.GetServiceListAsync(application.ApplicationName);
                    foreach (var service in services)
                    {
                        paymentLogger.LogInfo($"\tSERVICE - {service.ServiceTypeName}");
                        var partitions = await client.QueryManager.GetPartitionListAsync(service.ServiceName);
                        foreach (var partition in partitions)
                        {
                            paymentLogger.LogInfo($"\t\tPARTITION - {partition.PartitionInformation.Id}: {partition.PartitionInformation.Kind}");
                            if (partition.PartitionInformation.Kind == ServicePartitionKind.Int64Range)
                            {
                                var partitionInformation = (Int64RangePartitionInformation)partition.PartitionInformation;
                                paymentLogger.LogInfo($"\t\tRANGED PARTITION: {partitionInformation.LowKey} - {partitionInformation.HighKey}");
                                var actorServiceProxy = ServiceProxy.Create<IActorService>(service.ServiceName,
                                    new ServicePartitionKey(partitionInformation.LowKey));

                                ContinuationToken token = null;
                                var activeActors = new List<ActorInformation>();
                                var allActors = new List<ActorInformation>();

                                do
                                {
                                    var page = await actorServiceProxy.GetActorsAsync(token,
                                        default(CancellationToken));
                                    activeActors.AddRange(page.Items.Where(x => x.IsActive));
                                    allActors.AddRange(page.Items);
                                    token = page.ContinuationToken;
                                } while (token != null);
                                paymentLogger.LogInfo($"\t\t\t{activeActors.Count} active actors out of {allActors.Count} actors");
                            }
                        }
                    }
                }
            }
        }
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            paymentLogger.LogInfo("Creating Service Instance Listeners For LevyFundedProxyService");

            return new List<ServiceInstanceListener>
            {
                new ServiceInstanceListener(context =>listener = lifetimeScope.Resolve<IStatelessEndpointCommunicationListener>())
            };
        }
    }
}