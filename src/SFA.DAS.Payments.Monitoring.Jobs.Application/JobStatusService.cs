using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface IJobStatusService
    {
        Task<(bool Finished, DateTimeOffset? endTime)> JobStepsCompleted(long jobId);
    }

    public class JobStatusService : IJobStatusService
    {
        private readonly IJobsDataContext dataContext;
        private readonly IPaymentLogger logger;

        public JobStatusService(IJobsDataContext dataContext, IPaymentLogger logger)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(bool Finished, DateTimeOffset? endTime)> JobStepsCompleted(long jobId)
        {
            logger.LogDebug($"Now getting job steps status for job: {jobId}");
            var stepsStatus = await dataContext.GetJobStepsStatus(jobId);
            if (stepsStatus.ContainsKey(JobStepStatus.Queued) || stepsStatus.ContainsKey(JobStepStatus.Processing))
            {
                logger.LogDebug($"Not all job steps have finished processing for job: {jobId}.  There are still {stepsStatus.Keys.Count(key => key == JobStepStatus.Processing || key == JobStepStatus.Queued)} steps currently in progress.");
                return (false, null);
            }

            logger.LogVerbose($"Job has finished, now getting time of last job step for job {jobId}");
            var endTime = await dataContext.GetLastJobStepEndTime(jobId);
            var status = stepsStatus.Keys.Any(key => key == JobStepStatus.Failed)
                ? JobStatus.CompletedWithErrors
                : JobStatus.Completed;
            logger.LogDebug($"Got end time of last step for job: {jobId}, time: {endTime}. Status: {status}");
            await dataContext.SaveJobStatus(jobId,
                status,
                endTime);
            logger.LogInfo($"Finished recording completion status of job. Job: {jobId}, status: {status}, end time: {endTime}");
            return (true, endTime);
        }
    }
}