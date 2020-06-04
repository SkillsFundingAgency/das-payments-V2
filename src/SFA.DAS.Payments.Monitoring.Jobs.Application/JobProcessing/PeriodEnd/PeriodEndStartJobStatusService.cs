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

        public override async Task<bool> AnyOtherJobCriteriaMet(JobModel job, CancellationToken cancellationToken)
        {
            // Perform check
            if (await context.OutstandingJobsPresent(job.DcJobId, job.StartTime, cancellationToken))
            {
                return false;
            }
            return true;
        }

        protected override async Task<JobStatus> CompletedJobStatus(JobModel job,
            bool hasFailedMessages, CancellationToken cancellationToken)
        {
            if (await context.TimedOutJobsPresent(job.DcJobId, job.StartTime, cancellationToken))
            {
                return JobStatus.CompletedWithErrors;
            }

            return JobStatus.Completed;
        }
    }
}
