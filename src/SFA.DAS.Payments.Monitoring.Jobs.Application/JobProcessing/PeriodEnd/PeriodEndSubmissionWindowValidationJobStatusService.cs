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
    public interface IPeriodEndSubmissionWindowValidationJobStatusService : IPeriodEndJobStatusService
    {
    }

    public class PeriodEndSubmissionWindowValidationJobStatusService : PeriodEndJobStatusService, IPeriodEndSubmissionWindowValidationJobStatusService
    {
        private readonly IPaymentLogger logger;
        private readonly IMetricsValidationService metricsValidationService;

        public PeriodEndSubmissionWindowValidationJobStatusService(IJobStorageService jobStorageService, IPaymentLogger logger, ITelemetry telemetry, IJobStatusEventPublisher eventPublisher, IJobServiceConfiguration config, IJobsDataContext context, IMetricsValidationService metricsValidationService) : base(jobStorageService, logger, telemetry, eventPublisher, config)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.metricsValidationService = metricsValidationService ?? throw new ArgumentNullException(nameof(metricsValidationService));
        }

        protected override async Task<(bool IsComplete, JobStatus? OverriddenJobStatus, DateTimeOffset? completionTime)> PerformAdditionalJobChecks(JobModel job, CancellationToken cancellationToken)
        {
            logger.LogInfo($"Calling Metrics Validation Service for Job Id: {job.DcJobId}, Academic Year: {job.AcademicYear}, Collection Period: {job.CollectionPeriod}");
            var validationResult = await metricsValidationService.Validate(job.DcJobId.Value, job.AcademicYear, job.CollectionPeriod);
            logger.LogInfo($"Finished Calling Metrics Validation Service for Job Id: {job.DcJobId}, Academic Year: {job.AcademicYear}, Collection Period: {job.CollectionPeriod}. Result: {validationResult}");
            return ( true, validationResult ? JobStatus.Completed : JobStatus.CompletedWithErrors, DateTimeOffset.Now );
        }
    }
}
