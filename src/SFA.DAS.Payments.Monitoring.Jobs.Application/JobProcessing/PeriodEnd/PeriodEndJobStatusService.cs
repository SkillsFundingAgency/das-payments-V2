using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{

    public interface IPeriodEndJobStatusService : IJobStatusService { }

    public interface IPeriodEndStartedJobStatusService : IPeriodEndJobStatusService { }
    public class PeriodEndStartJobStatusService : PeriodEndJobStatusService
    {
        private readonly IJobsDataContext context;

        public PeriodEndStartJobStatusService(IJobStorageService jobStorageService, 
            IPaymentLogger logger, ITelemetry telemetry, IJobStatusEventPublisher eventPublisher, 
            IJobServiceConfiguration config, IJobsDataContext context) : base(jobStorageService, logger, telemetry, eventPublisher, config)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override async Task<bool> AnyOtherJobCriteriaMet(JobModel job, CancellationToken cancellationToken)
        {
            // Perform check
            if(await context.OutstandingJobsPresent(job.Id, job.StartTime, cancellationToken))
            {

                return false;
            }
            return true;
        }

        protected override async Task<JobStatus> CompletedJobStatus(JobModel job, 
            bool hasFailedMessages, CancellationToken cancellationToken)
        {
            if (await context.TimedOutJobsPresent(job.Id, job.StartTime, cancellationToken))
            {
                return JobStatus.CompletedWithErrors;
            }

            return JobStatus.Completed;
        }
    }

    public class PeriodEndJobStatusService : JobStatusService, IPeriodEndJobStatusService
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