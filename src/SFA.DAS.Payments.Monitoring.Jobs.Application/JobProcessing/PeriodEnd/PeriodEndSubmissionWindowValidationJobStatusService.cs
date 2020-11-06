using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{
    public interface IPeriodEndSubmissionWindowValidationJobStatusService : IPeriodEndJobStatusService
    {
    }

    public class PeriodEndSubmissionWindowValidationJobStatusService : PeriodEndJobStatusService, IPeriodEndSubmissionWindowValidationJobStatusService
    {
        private readonly IMetricsValidationService metricsValidationService;

        public PeriodEndSubmissionWindowValidationJobStatusService(IJobStorageService jobStorageService, IPaymentLogger logger, ITelemetry telemetry, IJobStatusEventPublisher eventPublisher, IJobServiceConfiguration config, IMetricsValidationService metricsValidationService) : base(jobStorageService, logger, telemetry, eventPublisher, config)
        {
            this.metricsValidationService = metricsValidationService ?? throw new ArgumentNullException(nameof(metricsValidationService));
        }

        public override async Task<bool> ManageStatus(long jobId, CancellationToken cancellationToken)
        {
            Logger.LogVerbose($"Now determining if job {jobId} has finished.");
            var job = await JobStorageService.GetJob(jobId, cancellationToken).ConfigureAwait(false);

            Logger.LogDebug("Now using Metrics Validation Service to determine if submissions window metrics are within tolerance.");
            var withinTolerance = await metricsValidationService.Validate(job.DcJobId.Value, job.AcademicYear, job.CollectionPeriod);

            if (withinTolerance)
                Logger.LogInfo("Submission window metrics are within tolerance.");
            else
                Logger.LogWarning("Submission window metrics are not within tolerance.");

            return await CompleteJob(job, withinTolerance ? JobStatus.Completed : JobStatus.CompletedWithErrors, DateTimeOffset.UtcNow, cancellationToken).ConfigureAwait(false);
        }
    }
}
