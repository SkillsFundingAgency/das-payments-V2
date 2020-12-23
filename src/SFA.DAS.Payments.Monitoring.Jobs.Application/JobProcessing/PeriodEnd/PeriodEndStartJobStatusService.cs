using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{
    public interface IPeriodEndStartJobStatusService : IPeriodEndJobStatusService { }
    public class PeriodEndStartJobStatusService : PeriodEndJobStatusService, IPeriodEndStartJobStatusService
    {
        private readonly IJobsDataContext context;

        public PeriodEndStartJobStatusService(IJobStorageService jobStorageService,
            IPaymentLogger logger, ITelemetry telemetry, IJobStatusEventPublisher eventPublisher,
            IJobServiceConfiguration config, IJobsDataContext context) : base(jobStorageService, logger, telemetry, eventPublisher, config)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected override async Task<(bool IsComplete, JobStatus? OverriddenJobStatus, DateTimeOffset? completionTime)> PerformAdditionalJobChecks(JobModel job, CancellationToken cancellationToken)
        {
            var outstandingJobs =
                await context.GetOutstandingOrTimedOutJobs(job.DcJobId, job.StartTime, cancellationToken);

            var timeoutsPresent = outstandingJobs.Any(x =>
                (x.JobStatus == JobStatus.TimedOut ||
                 x.JobStatus == JobStatus.DcTasksFailed) &&
                x.EndTime > job.StartTime);

            if (timeoutsPresent) //fail fast
            {
                return (true, JobStatus.CompletedWithErrors, outstandingJobs.Max(x => x.EndTime));
            }

            var processingJobsPresent = outstandingJobs.Any(x => x.JobStatus == JobStatus.InProgress || x.DcJobSucceeded == null);

            if (processingJobsPresent)
                return (false, (JobStatus?)null, (DateTimeOffset?)null);

            return (true, (JobStatus?)null, outstandingJobs.Max(x => x.EndTime));
        }
    }
}
