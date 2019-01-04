using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.JobsStatusService.Interfaces;

namespace SFA.DAS.Payments.Monitoring.JobsStatusService
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.None)]
    public class JobsStatusService : Actor, IJobsStatusService
    {
        private readonly IPaymentLogger logger;
        private readonly IJobStatusService jobStatusService;

        /// <summary>
        /// Initializes a new instance of JobStatusStatsService
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        /// <param name="logger"></param>
        /// <param name="jobStatusService"></param>
        public JobsStatusService(ActorService actorService, ActorId actorId, IPaymentLogger logger, IJobStatusService jobStatusService)
            : base(actorService, actorId)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.jobStatusService = jobStatusService ?? throw new ArgumentNullException(nameof(jobStatusService));
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return Task.CompletedTask;
        }

        public async Task<(bool Finished, DateTimeOffset? endTime)> JobStepsCompleted(long jobId)
        {
            logger.LogVerbose($"Performing job steps completed for job: {jobId}");
            var finished =  await jobStatusService.JobStepsCompleted(jobId);
            logger.LogDebug($"Finished performing job steps completed for job: {jobId}.  Finished: {finished}");
            return finished;
        }
    }
}
