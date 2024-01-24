using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{
    public interface IPeriodEndStartJobStatusService : IPeriodEndJobStatusService { }
    public class PeriodEndStartJobStatusService : BaseJobStatusService, IPeriodEndStartJobStatusService
    {
        private readonly IJobsDataContext context;
        private readonly IServiceStatusManager serviceStatusManager;
        protected override TimeSpan JobTimeoutPeriod => Config.PeriodEndStartJobTimeout;

        public PeriodEndStartJobStatusService(
            IJobStorageService jobStorageService,
            IPaymentLogger logger,
            ITelemetry telemetry,
            IJobStatusEventPublisher eventPublisher,
            IJobServiceConfiguration config,
            IJobsDataContext context, 
            IServiceStatusManager serviceStatusManager)
            : base(jobStorageService, logger, telemetry, eventPublisher, config)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.serviceStatusManager = serviceStatusManager ?? throw new ArgumentNullException(nameof(serviceStatusManager));
        }

        private void SendTelemetry(JobModel job, JobStatus status)
        {
            var properties = new Dictionary<string, string>
            {
                { TelemetryKeys.JobId, job.DcJobId.Value.ToString()},
                { TelemetryKeys.JobType, job.JobType.ToString("G")},
                { TelemetryKeys.Ukprn, job.Ukprn?.ToString() ?? string.Empty},
                { TelemetryKeys.InternalJobId, job.DcJobId.ToString()},
                { TelemetryKeys.CollectionPeriod, job.CollectionPeriod.ToString()},
                { TelemetryKeys.AcademicYear, job.AcademicYear.ToString()},
                { TelemetryKeys.Status, job.Status.ToString("G")}
            };

            Telemetry.TrackEvent($"PeriodEndStart Job Completed, Status: {status}", properties, new Dictionary<string, double>());
        }


        public async Task<bool> ManageStatus(long jobId, CancellationToken cancellationToken)
        {
            var job = await JobStorageService.GetJob(jobId, cancellationToken);
            if (job == null)
            {
                Logger.LogWarning($"Job not found: {jobId}");
                return false;
            }

            Logger.LogDebug($"checking if the DataLocks Approvals service is disabled.");
            if (!await serviceStatusManager.IsServiceRunning(ServiceNames.DataLocksApprovals.ApplicationName,
                    ServiceNames.DataLocksApprovals.ServiceName))
            {
                await CompleteJob(job, JobStatus.Completed, cancellationToken);
                Logger.LogInfo($"Data Locks Approvals Reference data service has now been stopped. Completed period end start job: {jobId}.");
                return true;
            }

            if (base.IsJobTimedOut(job,cancellationToken))
            {
                Logger.LogWarning($"Period end start job {jobId} has timed out.");
                await CompleteJob(job, JobStatus.TimedOut, cancellationToken);
                return true;
            }

            Logger.LogWarning($"The DataLocks Approvals reference data service is still running.");
            return false;
        }

        public async Task CompleteJob(JobModel job, JobStatus status, CancellationToken cancellationToken)
        {
            await JobStorageService.SaveJobStatus(job.DcJobId.Value, status, DateTimeOffset.UtcNow, cancellationToken);

            await EventPublisher.PeriodEndJobFinished(job, true);

            SendTelemetry(job);

        }
    }
}
