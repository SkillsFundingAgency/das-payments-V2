using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.JobsStatusService.Interfaces;

namespace SFA.DAS.Payments.Monitoring.JobsService.JobStatus
{
    public class JobsStatusServiceFacade : IJobsStatusServiceFacade
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger logger;

        public JobsStatusServiceFacade(IActorProxyFactory proxyFactory, IPaymentLogger logger)
        {
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(bool Finished, DateTimeOffset? endTime)> JobStepsCompleted(long jobId)
        {
            var actorId = new ActorId(jobId.ToString());
            var actor = proxyFactory.CreateActorProxy<IJobsStatusService>(new Uri("fabric:/SFA.DAS.Payments.Monitoring.ServiceFabric/JobsStatusServiceActorService"), actorId);
            try
            {
                return await actor.JobStepsCompleted(jobId);
            }
            catch (Exception e)
            {
                logger.LogError($"Error invoking the JobStatusStats actor service for job: {jobId}.  Error: {e.Message}", e);
                throw;
            }
        }
    }
}