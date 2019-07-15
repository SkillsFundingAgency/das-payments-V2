using System;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Model;
using SFA.DAS.Payments.PeriodEnd.Application.Infrastructure;

namespace SFA.DAS.Payments.PeriodEnd.Application.Services
{
    /// <summary>
    /// TODO: Temp solution to wait for jobs to finish
    /// </summary>
    [Obsolete("Temporary solution to wait for jobs to finish.  Should really use events from Job service but DC.JobContextManager doesn't support that kind of pattern")]
    public interface IJobStatusService
    {
        Task<bool> WaitForJobToFinish(long jobId);
    }

    /// <summary>
    /// TODO: Temp solution to wait for jobs to finish
    /// </summary>
    [Obsolete("Temporary solution to wait for jobs to finish.  Should really use events from Job service but DC.JobContextManager doesn't support that kind of pattern")]
    public class JobStatusService: IJobStatusService
    {
        private readonly IJobsDataContext dataContext;
        private readonly IPaymentLogger logger;
        private readonly IPeriodEndConfiguration config;

        public JobStatusService(IJobsDataContext dataContext, IPaymentLogger logger, IPeriodEndConfiguration config)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<bool> WaitForJobToFinish(long jobId)
        {
            //TODO: Temp brittle solution to wait for jobs to finish
            logger.LogDebug($"Waiting for job {jobId} to finish.");
            var endTime = DateTime.Now.Add(config.TimeToWaitForJobToComplete);
            while (DateTime.Now < endTime)
            {
                var job = await dataContext.GetJobByDcJobId(jobId).ConfigureAwait(false);
                if (job.Status == JobStatus.InProgress)
                {
                    logger.LogVerbose($"DC Job {jobId} is still in progress");
                    await Task.Delay(config.TimeToPauseBetweenChecks);
                    continue;
                }
                logger.LogInfo($"DC Job {jobId} finished. Status: {job.Status:G}.  Finish time: {job.EndTime:G}");
                return true;
            }
            logger.LogWarning($"Waiting {config.TimeToWaitForJobToComplete} but Job {jobId} still not finished.");
            return false;
        }
    }
}