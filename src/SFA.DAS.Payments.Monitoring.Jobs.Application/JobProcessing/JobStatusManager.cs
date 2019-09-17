using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing
{
    public interface IJobStatusManager
    {
        Task Start(CancellationToken cancellationToken);
        void StartMonitoringJob(long jobId, JobType jobType);
    }

    public class JobStatusManager : IJobStatusManager
    {
        private readonly IPaymentLogger logger;
        private readonly IUnitOfWorkScopeFactory scopeFactory;
        private readonly ConcurrentDictionary<long, JobType> currentJobs;
        private CancellationToken cancellationToken;
        private readonly TimeSpan interval;
        public JobStatusManager(IPaymentLogger logger, IUnitOfWorkScopeFactory scopeFactory, IJobServiceConfiguration configuration)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            interval = configuration.JobStatusInterval;
            currentJobs = new ConcurrentDictionary<long, JobType>();
        }

        public Task Start(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            return Run();
        }

        private async Task Run()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var tasks = currentJobs.Select(job => CheckJobStatus(job.Key)).ToList();
                await Task.WhenAll(tasks).ConfigureAwait(false);
                await Task.Delay(interval, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task CheckJobStatus(long jobId)
        {
            try
            {
                using (var scope = scopeFactory.Create($"CheckJobStatus:{jobId}"))
                {
                    try
                    {
                        var jobStatusService = scope.Resolve<IJobStatusService>();
                        await jobStatusService.ManageStatus(jobId, cancellationToken).ConfigureAwait(false);
                        await scope.Commit();
                    }
                    catch (Exception ex)
                    {
                        scope.Abort();
                        logger.LogWarning($"Failed to update job status for job: {jobId}, Error: {ex.Message}. {ex}");
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to create or abort the scope of the state manager transaction for: {jobId}. Error: {e.Message}", e);
                throw;
            }
        }

        public void StartMonitoringJob(long jobId, JobType jobType)
        {
            currentJobs.AddOrUpdate(jobId,jobType,(key,currentJobType)=> jobType);
        }
    }
}