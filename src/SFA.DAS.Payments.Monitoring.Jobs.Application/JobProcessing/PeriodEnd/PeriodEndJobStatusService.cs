using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{

    public interface IPeriodEndJobStatusService: IJobStatusService{ }

    public class PeriodEndJobStatusService: JobStatusService, IPeriodEndJobStatusService
    {
        public PeriodEndJobStatusService(IJobStorageService jobStorageService, IPaymentLogger logger, ITelemetry telemetry, IJobStatusEventPublisher eventPublisher, IJobServiceConfiguration config) : base(jobStorageService, logger, telemetry, eventPublisher, config)
        {
        }

        protected override  Task<bool> CheckSavedJobStatus(JobModel job, CancellationToken cancellationToken)
        {
                return Task.FromResult(false);
        }

        protected override async Task<bool> CompleteJob(JobModel job, JobStatus status, DateTimeOffset endTime, CancellationToken cancellationToken)
        {
            var isComplete = await base.CompleteJob(job, status, endTime, cancellationToken);

            if(isComplete && job.Status != JobStatus.TimedOut)
             await EventPublisher.PeriodEndJobFinished(job, true);

            return isComplete;
        }

        protected override async Task<bool> IsJobTimedOut(JobModel job, CancellationToken cancellationToken)
        {
            var isJobTimedOut = await base.IsJobTimedOut(job, cancellationToken);
            if (isJobTimedOut)
            {
                await EventPublisher.PeriodEndJobFinished(job, false);
            }

            return isJobTimedOut;
        }
    }
}