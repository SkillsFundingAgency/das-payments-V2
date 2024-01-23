using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{

    public interface IPeriodEndJobStatusService : IJobStatusService { }

    public class PeriodEndJobStatusService : JobStatusService, IPeriodEndJobStatusService
    {
        protected override TimeSpan JobTimeoutPeriod => Config.PeriodEndRunJobTimeout;

        public PeriodEndJobStatusService(IJobStorageService jobStorageService, IPaymentLogger logger, ITelemetry telemetry, IJobStatusEventPublisher eventPublisher, IJobServiceConfiguration config) : base(jobStorageService, logger, telemetry, eventPublisher, config)
        {
        }

        protected override Task<bool> CheckSavedJobStatus(JobModel job, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        protected override async Task<bool> CompleteJob(JobModel job, JobStatus status, DateTimeOffset endTime, CancellationToken cancellationToken)
        {
            Logger.LogDebug($"Completing PeriodEndJob Job {job.DcJobId}.");

            if (!await base.CompleteJob(job, status, endTime, cancellationToken))
                return false;

            await EventPublisher.PeriodEndJobFinished(job, job.Status == JobStatus.Completed);
            Logger.LogInfo($"Completed PeriodEndJob Job {job.DcJobId}.");

            return true;
        }
    }
}