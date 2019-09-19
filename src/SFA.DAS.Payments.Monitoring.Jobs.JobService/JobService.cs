using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.JobService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService
{
    [StatePersistence(StatePersistence.Volatile)]
    public class JobService : StatefulService, IJobService, IService
    {
        private readonly IPaymentLogger logger;
        private readonly IUnitOfWorkScopeFactory scopeFactory;
        private readonly IJobStatusManager jobStatusManager;
        private readonly ILifetimeScope lifetimeScope;
        private IStatefulEndpointCommunicationListener listener;

        public JobService(StatefulServiceContext context, IPaymentLogger logger, IUnitOfWorkScopeFactory scopeFactory, IJobStatusManager jobStatusManager, ILifetimeScope lifetimeScope)
            : base(context)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            this.jobStatusManager = jobStatusManager ?? throw new ArgumentNullException(nameof(jobStatusManager));
            this.lifetimeScope = lifetimeScope;
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            EndpointConfigurationEvents.ConfiguringEndpointName += (object sender, Payments.Application.Infrastructure.Ioc.Modules.EndpointName e) =>
            {
                e.Name += ((NamedPartitionInformation)Partition.PartitionInfo).Name;
            };

            var serviceListener = new ServiceReplicaListener(context =>
                listener = lifetimeScope.Resolve<IStatefulEndpointCommunicationListener>());


            return new List<ServiceReplicaListener>
            {
                serviceListener,
            };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await jobStatusManager.Start(cancellationToken);
        }

        public async Task RecordEarningsJob(RecordEarningsJob message, CancellationToken cancellationToken)
        {
            using (var scope = scopeFactory.Create("JobService.RecordEarningsJob"))
            {
                try
                {
                    var earningsJobService = scope.Resolve<IEarningsJobService>();
                    await earningsJobService.RecordNewJob(message, cancellationToken).ConfigureAwait(false);
                    await scope.Commit();
                }
                catch (Exception ex)
                {
                    scope.Abort();
                    logger.LogWarning($"Failed to record earnings job: {message.JobId}. Error: {ex.Message}, {ex}");
                    throw;
                }
            }
            jobStatusManager.StartMonitoringJob(message.JobId, JobType.EarningsJob);
        }

        public async Task RecordEarningsJobAdditionalMessages(RecordEarningsJobAdditionalMessages message,
            CancellationToken cancellationToken)
        {
            using (var scope = scopeFactory.Create("JobService.RecordEarningsJobAdditionalMessages"))
            {
                try
                {
                    var earningsJobService = scope.Resolve<IEarningsJobService>();
                    await earningsJobService.RecordNewJobAdditionalMessages(message, cancellationToken).ConfigureAwait(false);
                    await scope.Commit();
                }
                catch (Exception ex)
                {
                    scope.Abort();
                    logger.LogWarning($"Failed to record earnings job: {message.JobId}. Error: {ex.Message}, {ex}");
                    throw;
                }
            }
        }

        public async Task RecordJobMessageProcessingStatus(RecordJobMessageProcessingStatus message, CancellationToken cancellationToken)
        {
            using (var scope = scopeFactory.Create("JobService.RecordJobMessageProcessingStatus"))
            {
                try
                {
                    var jobMessageService = scope.Resolve<IJobMessageService>();
                    await jobMessageService.RecordCompletedJobMessageStatus(message, cancellationToken).ConfigureAwait(false);
                    await scope.Commit();
                }
                catch (Exception ex)
                {
                    scope.Abort();
                    logger.LogWarning($"Failed to record earnings job: {message.JobId}. Error: {ex.Message}, {ex}");
                    throw;
                }
            }
        }

        public Task RecordJobMessageProcessingStartedStatus(RecordStartedProcessingJobMessages message,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
