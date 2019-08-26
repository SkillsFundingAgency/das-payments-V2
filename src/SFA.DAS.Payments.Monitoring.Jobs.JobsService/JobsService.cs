using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.JobsService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsService
{

    [StatePersistence(StatePersistence.Volatile)]
    public class JobsService : Actor, IJobsService
    {
        private readonly ILifetimeScope lifetimeScope;

        public JobsService(ILifetimeScope lifetimeScope, ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        public async Task RecordEarningsJob(RecordEarningsJob message)
        {
            var service = lifetimeScope.Resolve<IEarningsJobService>();
            await service.JobStarted(message, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<JobStatus> RecordJobMessageProcessingStatus(RecordJobMessageProcessingStatus message)
        {
            var jobMessageService = lifetimeScope.Resolve<IJobMessageService>();
            await jobMessageService.JobMessageCompleted(message, CancellationToken.None).ConfigureAwait(false);
            var statusService = lifetimeScope.Resolve<IJobStatusService>();
            return await statusService.ManageStatus(CancellationToken.None);
        }
    }
}
