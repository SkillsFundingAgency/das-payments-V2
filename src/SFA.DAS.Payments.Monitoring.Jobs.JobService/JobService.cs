using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.JobService.Interfaces;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService
{
    [StatePersistence(StatePersistence.Volatile)]
    public class JobService : StatefulService, IJobService
    {
        private readonly IPaymentLogger logger;
        private readonly IJobStatusManager jobStatusManager;
        private readonly ILifetimeScope lifetimeScope;

        public JobService(StatefulServiceContext context, IPaymentLogger logger, IJobStatusManager jobStatusManager, ILifetimeScope lifetimeScope)
            : base(context)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.jobStatusManager = jobStatusManager ?? throw new ArgumentNullException(nameof(jobStatusManager));
            this.lifetimeScope = lifetimeScope;
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            try
            {
                var partitionEndpointName = ((NamedPartitionInformation) Partition.PartitionInfo).Name;
                var batchListener = lifetimeScope.Resolve<IServiceBusBatchCommunicationListener>();
                batchListener.EndpointName += partitionEndpointName;
                var serviceListener = new ServiceReplicaListener(context => batchListener);
                return new List<ServiceReplicaListener>
                {
                    serviceListener
                };
            }
            catch (Exception e)
            {
                logger.LogError($"Error: {e.Message}",e);
                throw;
            }
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.WhenAll(RunSendOnlyEndpoint(), jobStatusManager.Start(cancellationToken));
        }

        private async Task RunSendOnlyEndpoint()
        {
            var endpoint = lifetimeScope.Resolve<EndpointConfiguration>();
            //endpoint.SendOnly();
            var factory = lifetimeScope.Resolve<IEndpointInstanceFactory>();
            await factory.GetEndpointInstance();
        }
    }
}
