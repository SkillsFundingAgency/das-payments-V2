using System;
using System.Collections.Generic;
using System.Linq;
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
        protected override TimeSpan JobTimeoutPeriod => Config.PeriodEndRunJobTimeout;

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

        private void SendTelemetry(JobModel job, List<OutstandingJobResult> processingJobsPresent = null, List<long?> jobsWithoutSubmissionSummariesPresent = null)
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

            if (processingJobsPresent != null)
            {
                properties.Add("InProgressJobsCount", processingJobsPresent.Count.ToString());
                properties.Add("InProgressJobsList", string.Join(", ", processingJobsPresent.Select(j => j.ToJson())));
            }


            if (jobsWithoutSubmissionSummariesPresent != null)
            {
                properties.Add("jobsWithoutSubmissionSummariesCount", jobsWithoutSubmissionSummariesPresent.Count.ToString());
                properties.Add("jobsWithoutSubmissionSummaries", string.Join(", ", jobsWithoutSubmissionSummariesPresent.Select(j => j.ToJson())));
            }

            Telemetry.TrackEvent("PeriodEndStart Job Status Update", properties, new Dictionary<string, double>());
        }


        public Task<bool> ManageStatus(long jobId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
