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
        private readonly ConcurrentDictionary<long, JobStatus> currentJobs;
        private CancellationToken cancellationToken;
        private readonly TimeSpan interval;
        public JobStatusManager(IPaymentLogger logger, IUnitOfWorkScopeFactory scopeFactory, IJobServiceConfiguration configuration)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            interval = configuration.JobStatusInterval;
            currentJobs = new ConcurrentDictionary<long, JobStatus>();
        }

        public Task Start(CancellationToken suppliedCancellationToken)
        {
            this.cancellationToken = suppliedCancellationToken;
            return Run();
        }

        private async Task Run()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var tasks = currentJobs.Select(job => CheckJobStatus(job.Key)).ToList();
                await Task.WhenAll(tasks).ConfigureAwait(false);
                var completedJobs = currentJobs.Where(item => item.Value != JobStatus.InProgress).ToList();
                foreach (var completedJob in completedJobs)
                {
                    if (!currentJobs.TryRemove(completedJob.Key, out _))
                        logger.LogWarning($"Couldn't remove completed job from jobs list.  JOb: {completedJob.Key}, status: {completedJob.Value}");
                }
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
                        var status = await jobStatusService.ManageStatus(jobId, cancellationToken).ConfigureAwait(false);
                        await scope.Commit();
                        currentJobs[jobId] = status;
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
            currentJobs.AddOrUpdate(jobId, JobStatus.InProgress, (key, status) => status);
        }
    }
}