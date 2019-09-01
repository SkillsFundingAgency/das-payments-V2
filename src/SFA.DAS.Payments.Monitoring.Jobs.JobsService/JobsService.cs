using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Core;
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
        private readonly ITelemetry telemetry;
        private IActorTimer jobCleanUpTimer;

        private const int MaxIdleMinutesForJob = 10;
        private const int ReminderIntervalInMinutesForJob = 1;

        public JobsService(IPaymentLogger logger, ILifetimeScope lifetimeScope, ActorService actorService, ActorId actorId, ITelemetry telemetry) 
            : base(actorService, actorId)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        protected override Task OnActivateAsync()
        {
            jobCleanUpTimer = RegisterTimer(CleanUpJob, null, TimeSpan.FromMinutes(MaxIdleMinutesForJob), TimeSpan.FromMinutes(ReminderIntervalInMinutesForJob));
            return base.OnActivateAsync();
        }

        protected override Task OnDeactivateAsync()
        {
            if (jobCleanUpTimer != null)
            {
                UnregisterTimer(jobCleanUpTimer);
            }

            return base.OnDeactivateAsync();
        }

        private async Task<DateTimeOffset?> GetLastMessageStatusTme()
        {
            var item = await StateManager.TryGetStateAsync<DateTimeOffset>("last_message_time").ConfigureAwait(false);
            return item.HasValue ? item.Value : (DateTimeOffset?)null;
        }

        private async Task SetLastMesasgeStatusTime(DateTimeOffset lastMessageTime)
        {
            await StateManager.AddOrUpdateStateAsync < DateTimeOffset>("last_message_time", lastMessageTime, (oldKey, oldValue) => lastMessageTime).ConfigureAwait(false);
        }

        protected async Task CleanUpJob(object state)
        {
            var lastMessageTime = await GetLastMessageStatusTme().ConfigureAwait(false);
            if (lastMessageTime == null || lastMessageTime.Value.AddMinutes(MaxIdleMinutesForJob) > DateTimeOffset.UtcNow)
            {
                logger.LogVerbose(lastMessageTime.HasValue
                    ? $"Job timeout not triggered.  Is due to timeout at: {lastMessageTime.Value.AddMinutes(MaxIdleMinutesForJob)}"
                    : "No job messages have been received so cannot start job timeout.");
                return;
            }
            logger.LogInfo($"Job has timed out due to max idle time. Job: {Id.GetStringId()}");
            var statusService = lifetimeScope.Resolve<IJobStatusService>();
            await statusService.StopJob();
        }

        public async Task RecordEarningsJob(RecordEarningsJob message, CancellationToken cancellationToken)
        {
            try
            {
                using (var operation = telemetry.StartOperation("JobsService.RecordEarningsJob", message.CommandId.ToString()))
                {
                    var stopwatch = Stopwatch.StartNew();
                    var service = lifetimeScope.Resolve<IEarningsJobService>();
                    await service.JobStarted(message, CancellationToken.None).ConfigureAwait(false);
                    telemetry.TrackDuration("JobsService.RecordEarningsJob", stopwatch.Elapsed);
                    telemetry.StopOperation(operation);
                }

                await SetLastMesasgeStatusTime(DateTimeOffset.Now).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.LogWarning($"Error recording earning job. Job: {message.JobId}, ukprn: {message.Ukprn}, Error: {e.Message}. {e}");
                throw;
            }
        }

        public async Task<JobStatus> RecordJobMessageProcessingStatus(RecordJobMessageProcessingStatus message, CancellationToken cancellationToken)
        {
            try
            {
                using (var operation = telemetry.StartOperation("JobsService.RecordJobMessageProcessingStatus", message.CommandId.ToString()))
                {
                    var stopwatch = Stopwatch.StartNew();
                    //TODO: use statemanager tx and make sure IActorDataCache uses current tx too
                    var jobMessageService = lifetimeScope.Resolve<IJobMessageService>();
                    await jobMessageService.RecordCompletedJobMessageStatus(message, CancellationToken.None).ConfigureAwait(false);
                    var statusService = lifetimeScope.Resolve<IJobStatusService>();
                    var status = JobStatus.InProgress;
                    if (message.AllowJobCompletion)
                    {
                        status = await statusService.ManageStatus(CancellationToken.None);
                        if (status != JobStatus.InProgress)
                            await StateManager.ClearCacheAsync(CancellationToken.None);
                    }
                    telemetry.TrackDuration("JobsService.RecordJobMessageProcessingStatus", stopwatch.Elapsed);
                    telemetry.StopOperation(operation);
                    await SetLastMesasgeStatusTime(DateTimeOffset.Now).ConfigureAwait(false);
                    return status;
                }
            }
            catch (Exception e)
            {
                logger.LogWarning($"Error recording job message status. Job: {message.JobId}, message id: {message.Id}, name: {message.MessageName}.  Error: {e.Message}. {e}");
                throw;
            }
        }

        public async Task<JobStatus> RecordJobMessageProcessingStartedStatus(RecordStartedProcessingJobMessages message, CancellationToken cancellationToken)
        {
            try
            {
                using (var operation = telemetry.StartOperation("JobsService.RecordJobMessageProcessingStartedStatus", message.CommandId.ToString()))
                {
                    var stopwatch = Stopwatch.StartNew();
                    //TODO: use statemanager tx and make sure IActorDataCache uses current tx too
                    var jobMessageService = lifetimeScope.Resolve<IJobMessageService>();
                    await jobMessageService.RecordStartedJobMessages(message, CancellationToken.None).ConfigureAwait(false);
                    var statusService = lifetimeScope.Resolve<IJobStatusService>();
                    var status = await statusService.ManageStatus(CancellationToken.None);
                    if (status != JobStatus.InProgress)
                        await StateManager.ClearCacheAsync(CancellationToken.None);
                    telemetry.TrackDuration("JobsService.RecordJobMessageProcessingStartedStatus", stopwatch.Elapsed);
                    telemetry.StopOperation(operation);
                    await SetLastMesasgeStatusTime(DateTimeOffset.Now).ConfigureAwait(false);
                    return status;
                }
            }
            catch (Exception e)
            {
                logger.LogWarning($"Error recording job messages started. Job: {message.JobId}, message: {message.ToJson()}.  Error: {e.Message}. {e}");
                throw;
            }
        }

        
    }
}
