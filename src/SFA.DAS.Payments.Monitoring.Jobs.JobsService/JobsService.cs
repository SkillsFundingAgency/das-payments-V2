using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.JobsService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsService
{

    [StatePersistence(StatePersistence.Volatile)]
    public class JobsService : Actor, IJobsService
    {
        private readonly IPaymentLogger logger;
        private readonly ILifetimeScope lifetimeScope;

        public JobsService(IPaymentLogger logger, ILifetimeScope lifetimeScope, ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        public async Task RecordEarningsJob(RecordEarningsJob message)
        {
            try
            {
                var service = lifetimeScope.Resolve<IEarningsJobService>();
                await service.JobStarted(message, CancellationToken.None).ConfigureAwait(false);

            }
            catch (Exception e)
            {
                logger.LogWarning($"Error recording earning job. Job: {message.JobId}, ukprn: {message.Ukprn}, Error: {e.Message}. {e}");
                throw;
            }
        }

        public async Task<JobStatus> RecordJobMessageProcessingStatus(RecordJobMessageProcessingStatus message)
        {
            try
            {
                var jobMessageService = lifetimeScope.Resolve<IJobMessageService>();
                await jobMessageService.JobMessageCompleted(message, CancellationToken.None).ConfigureAwait(false);
                var statusService = lifetimeScope.Resolve<IJobStatusService>();
                return await statusService.ManageStatus(CancellationToken.None);
            }
            catch (Exception e)
            {
                logger.LogWarning($"Error recording job message status. Job: {message.JobId}, message id: {message.Id}, name: {message.MessageName}.  Error: {e.Message}. {e}");
                throw;
            }
        }
    }
}
