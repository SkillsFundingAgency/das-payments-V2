using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Data;

namespace SFA.DAS.Payments.JobContextMessageHandling.JobStatus
{
    /// <summary>
    /// TODO: Temp solution to wait for jobs to finish
    /// </summary>
    [Obsolete("Temporary solution to wait for jobs to finish.  Should really use events from Job service but DC.JobContextManager doesn't support that kind of pattern")]
    public interface IJobStatusService
    {
        Task<bool> WaitForJobToFinish(long jobId, CancellationToken cancellationToken, bool isPeriodEndRun = false);
        Task<bool> JobCurrentlyRunning(long jobId);
        Task<bool> WaitForPeriodEndStartedToFinish(long dcJobId, CancellationToken cancellationToken);
        Task<bool> WaitForPeriodEndSubmissionWindowValidationToFinish(long dcJobId, CancellationToken cancellationToken);
        Task<bool> WaitForPeriodEndRunJobToFinish(long jobId, CancellationToken cancellationToken);
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

        public async Task<bool> WaitForJobToFinish(long jobId, CancellationToken cancellationToken, bool isPeriodEndRun = false)
        {
            //TODO: Temp brittle solution to wait for jobs to finish
            logger.LogDebug($"Waiting for job {jobId} to finish.");
            var endTime = isPeriodEndRun ? DateTime.Now.Add(config.TimeToWaitForPeriodEndRunJobToComplete) : DateTime.Now.Add(config.TimeToWaitForJobToComplete);
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
            }
            logger.LogWarning($"Waiting {(isPeriodEndRun ? config.TimeToWaitForPeriodEndRunJobToComplete : config.TimeToWaitForJobToComplete)} but Job {jobId} still not finished.");
            return false;
        }

        public async Task<bool> WaitForPeriodEndRunJobToFinish(long jobId, CancellationToken cancellationToken)
        {
            return await WaitForJobToFinish(jobId, cancellationToken, true);
        }

        public async Task<bool> JobCurrentlyRunning(long jobId)
        {
            var job = await dataContext.GetJobByDcJobId(jobId).ConfigureAwait(false);
            return job != null && job.Status == Monitoring.Jobs.Model.JobStatus.InProgress;
        }

        public async Task<bool> WaitForPeriodEndStartedToFinish(long dcJobId, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Waiting for job with Dc JobId: {dcJobId} to finish.");
            var endTime = DateTime.Now.Add(config.TimeToWaitForJobToComplete);
            while (DateTime.Now < endTime)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var job = await dataContext.GetJobByDcJobId(dcJobId).ConfigureAwait(false);
                if (job != null && (job.EndTime != null ||
                                    job.Status != Monitoring.Jobs.Model.JobStatus.InProgress))
                {
                    logger.LogInfo($"DC Job {dcJobId} finished. Status: {job.Status:G}.  Finish time: {job.EndTime:G}");
                    return job.Status == Monitoring.Jobs.Model.JobStatus.Completed;
                }
                logger.LogVerbose($"DC Job {dcJobId} is still in progress");
                await Task.Delay(config.TimeToPauseBetweenChecks, cancellationToken);
            }
            logger.LogWarning($"Waiting {config.TimeToWaitForJobToComplete} but Job {dcJobId} still not finished.");
            return false;
        }

        public async Task<bool> WaitForPeriodEndSubmissionWindowValidationToFinish(long jobId, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Waiting for job with JobId: {jobId} to finish.");
            var endTime = DateTime.Now.Add(config.TimeToWaitForJobToComplete);
            while (DateTime.Now < endTime)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var job = await dataContext.GetJobByDcJobId(jobId);
                if (job != null && (job.EndTime != null ||
                                    job.Status != Monitoring.Jobs.Model.JobStatus.InProgress))
                {
                    logger.LogInfo($"Job {jobId} finished. Status: {job.Status:G}.  Finish time: {job.EndTime:G}");
                    return job.Status == Monitoring.Jobs.Model.JobStatus.Completed;
                }
                logger.LogVerbose($"Job {jobId} is still in progress");
                await Task.Delay(config.TimeToPauseBetweenChecks, cancellationToken);
            }
            logger.LogWarning($"Waiting {config.TimeToWaitForJobToComplete} but Job {jobId} still not finished.");
            return false;
        }
    }
}
