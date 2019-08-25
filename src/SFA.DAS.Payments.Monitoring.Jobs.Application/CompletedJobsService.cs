using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface ICompletedJobsService
    {
        Task UpdateCompletedJobs(CancellationToken cancellationToken);
    }

    public class CompletedJobsService : ICompletedJobsService
    {
        private readonly IJobsDataContext dataContext;
        private readonly IPaymentLogger logger;
        private readonly IContainerScopeFactory scopeFactory;
        private readonly ITelemetry telemetry;

        public CompletedJobsService(IJobsDataContext dataContext, IPaymentLogger logger, IContainerScopeFactory scopeFactory, ITelemetry telemetry)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task UpdateCompletedJobs(CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Getting jobs still in progress.");
            var jobs = await dataContext.GetInProgressJobs();
            if (jobs.Count == 0)
            {
                logger.LogDebug($"No in-progress jobs found.");
                telemetry.TrackEvent("InProgress Jobs", 0); //TODO: maybe this should be a metric rather than an event
                return;
            }
            logger.LogVerbose($"Got {jobs.Count} in-progress jobs.");
            foreach (var job in jobs)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        logger.LogInfo("Stopping job completion run. Cancellation requested.");
                        return;
                    }
                    logger.LogVerbose($"Attempting to update job status for job: {job.Id}");
                    using (var scope = scopeFactory.CreateScope())
                    {
                        var jobStatusService = scope.Resolve<IJobStatusService>();
                        await jobStatusService.UpdateStatus(job, cancellationToken);
                    }
                    logger.LogDebug($"Finished attempting to update job status for job: {job.Id}");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Failed to update the status of job: {job.Id}. Error: {ex.Message}", ex);
                    throw;
                }
            }

            var inProgressCount = jobs.Count(job => job.Status == JobStatus.InProgress);
            telemetry.TrackEvent("InProgress Jobs", inProgressCount); //TODO: maybe this should be a metric rather than an event
        }
    }
}