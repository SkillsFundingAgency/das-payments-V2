using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Query;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ServiceFabric.Core;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, 
    RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace SFA.DAS.Payments.EarningEvents.EarningEventsService
{
    public class EarningEventsService : StatelessService
    {
        private readonly ILifetimeScope lifetimeScope;
        private IJobContextManager<JobContextMessage> jobContextManager;
        private readonly IPaymentLogger logger;

        public EarningEventsService(StatelessServiceContext context, ILifetimeScope lifetimeScope, IPaymentLogger logger)
            : base(context)
        {
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new List<ServiceInstanceListener>
            {
                new ServiceInstanceListener(context =>lifetimeScope.Resolve<IStatelessEndpointCommunicationListener>())
            };
        }

        private async Task LogActorInformation()
        {
            using (var client = new FabricClient())
            {
                var applications = await client.QueryManager.GetApplicationListAsync();

                foreach (var application in applications)
                {
                    logger.LogInfo($"APPLICATION - {application.ApplicationTypeName}");
                    var services = await client.QueryManager.GetServiceListAsync(application.ApplicationName);
                    foreach (var service in services)
                    {
                        logger.LogInfo($"\tSERVICE - {service.ServiceTypeName}");
                        var partitions = await client.QueryManager.GetPartitionListAsync(service.ServiceName);
                        foreach (var partition in partitions)
                        {
                            logger.LogInfo($"\t\tPARTITION - {partition.PartitionInformation.Id}: {partition.PartitionInformation.Kind}");
                            if (partition.PartitionInformation.Kind == ServicePartitionKind.Int64Range)
                            {
                                var partitionInformation = (Int64RangePartitionInformation)partition.PartitionInformation;
                                logger.LogInfo($"\t\tRANGED PARTITION: {partitionInformation.LowKey} - {partitionInformation.HighKey}");
                                var actorServiceProxy = ServiceProxy.Create<IActorService>(service.ServiceName,
                                    new ServicePartitionKey(partitionInformation.LowKey));

                                ContinuationToken token = null;
                                var activeActors = new List<ActorInformation>();
                                var allActors = new List<ActorInformation>();

                                do
                                {
                                    try
                                    {
                                        var page = await actorServiceProxy.GetActorsAsync(token,
                                            default(CancellationToken));
                                        activeActors.AddRange(page.Items.Where(x => x.IsActive));
                                        allActors.AddRange(page.Items);
                                        token = page.ContinuationToken;
                                    }
                                    catch (Exception e)
                                    {
                                        logger.LogError("Error enumerating actors", e);
                                        token = null;
                                    }
                                } while (token != null);
                                logger.LogInfo($"\t\t\t{activeActors.Count} active actors out of {allActors.Count} actors");
                            }
                        }
                    }
                }
            }
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var initialised = false;

            try
            {
                logger.LogDebug("Starting the Earning Events service.");
                jobContextManager = lifetimeScope.Resolve<IJobContextManager<JobContextMessage>>();
                jobContextManager.OpenAsync(cancellationToken);
                initialised = true;
                logger.LogInfo("Started the Earning Events service.");
                while (true)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                    await LogActorInformation();
                }
            }
            catch (Exception exception) when (!(exception is TaskCanceledException))
            {
                // Ignore, as an exception is only really thrown on cancellation of the token.
                logger.LogError($"Reference Data Stateless Service Exception. Error: {exception}.", exception);
            }
            finally
            {
                if (initialised)
                {
                    logger.LogInfo("Earning Events Stateless Service End");
                    await jobContextManager.CloseAsync();
                }
            }
        }
    }
}