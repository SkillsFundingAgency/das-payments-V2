using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.JobContextMessageHandling.Infrastructure;
using SFA.DAS.Payments.Monitoring.Jobs.Data;

namespace SFA.DAS.Payments.JobContextMessageHandling.JobStatus
{
    /// <summary>
    /// TODO: Temp solution to wait for jobs to finish
    /// </summary>
    [Obsolete("Temporary solution to wait for jobs to finish.  Should really use events from Job service but DC.JobContextManager doesn't support that kind of pattern")]
    public interface IJobStatusService
    {
        Task<bool> WaitForJobToFinish(long jobId, CancellationToken cancellationToken);
        Task<bool> JobCurrentlyRunning(long jobId);
        Task<bool> WaitForPeriodEndStartedToFinish(long jobId, CancellationToken cancellationToken);
    }

    /// <summary>
    /// TODO: Temp solution to wait for jobs to finish
    /// </summary>
    [Obsolete("Temporary solution to wait for jobs to finish.  Should really use events from Job service but DC.JobContextManager doesn't support that kind of pattern")]
    public class JobStatusService : IJobStatusService
    {
        private readonly IJobsDataContext dataContext;
        private readonly IPaymentLogger logger;
        private readonly IJobStatusConfiguration config;

        public JobStatusService(IJobsDataContext dataContext, IPaymentLogger logger, IJobStatusConfiguration config)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<bool> WaitForJobToFinish(long jobId, CancellationToken cancellationToken)
        {
            //TODO: Temp brittle solution to wait for jobs to finish
            logger.LogDebug($"Waiting for job {jobId} to finish.");
            var endTime = DateTime.Now.Add(config.TimeToWaitForJobToComplete);
            while (DateTime.Now < endTime)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var job = await dataContext.GetJobByDcJobId(jobId).ConfigureAwait(false);
                if (job != null && (job.DataLocksCompletionTime != null ||
                                    job.Status != Monitoring.Jobs.Model.JobStatus.InProgress))
                {
                    logger.LogInfo($"DC Job {jobId} finished. Status: {job.Status:G}.  Finish time: {job.EndTime:G}");
                    return true;
                }
                logger.LogVerbose($"DC Job {jobId} is still in progress");
                await Task.Delay(config.TimeToPauseBetweenChecks);
                continue;
            }
            logger.LogWarning($"Waiting {config.TimeToWaitForJobToComplete} but Job {jobId} still not finished.");
            return false;
        }

        public async Task<bool> JobCurrentlyRunning(long jobId)
        {
            var job = await dataContext.GetJobByDcJobId(jobId).ConfigureAwait(false);
            return job != null && job.Status == Monitoring.Jobs.Model.JobStatus.InProgress;
        }

        public async Task<bool> WaitForPeriodEndStartedToFinish(long jobId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}