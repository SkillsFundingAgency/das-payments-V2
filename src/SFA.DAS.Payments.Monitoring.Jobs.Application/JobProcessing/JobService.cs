using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing
{
    public abstract class JobService
    {
        protected readonly IPaymentLogger logger;
        protected readonly IJobStorageService jobStorageService;
        protected readonly ITelemetry telemetry;

        protected JobService(IPaymentLogger logger, IJobStorageService jobStorageService, ITelemetry telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.jobStorageService = jobStorageService ?? throw new ArgumentNullException(nameof(jobStorageService));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        protected virtual async Task RecordNewJob(JobModel jobDetails, List<GeneratedMessage> generatedMessages, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!jobDetails.DcJobId.HasValue)
                throw new InvalidOperationException("No DcJob Id found on the job.");

            var isNewJob = await jobStorageService.StoreNewJob(jobDetails, CancellationToken.None);
            logger.LogDebug($"Finished storing new job: {jobDetails.Id} for dc job id: {jobDetails.DcJobId}. Now storing the job messages.");
            await RecordJobInProgressMessages(jobDetails.DcJobId.Value, generatedMessages, cancellationToken).ConfigureAwait(false);
            logger.LogDebug($"Finished storing new job messages for job: {jobDetails.Id} for dc job id: {jobDetails.DcJobId}.");
            if (isNewJob)
                SendTelemetry(jobDetails);
            logger.LogInfo($"Finished saving the job info.  DC Job Id: {jobDetails.DcJobId}");
        }

        protected async Task RecordJobInProgressMessages(long jobId, List<GeneratedMessage> generatedMessages, CancellationToken cancellationToken)
        {
            await jobStorageService.StoreInProgressMessages(jobId,
                generatedMessages.Select(message => new InProgressMessage
                {
                    MessageId = message.MessageId,
                    JobId = jobId,
                    MessageName = message.MessageName
                }).ToList(), cancellationToken);
        }


        private void SendTelemetry(JobModel jobDetails)
        {
            telemetry.TrackEvent("Started Job",
                new Dictionary<string, string>
                {
                    { TelemetryKeys.JobType, jobDetails.JobType.ToString("G") },
                    { TelemetryKeys.Ukprn, (jobDetails.Ukprn ?? 0).ToString() },
                    { TelemetryKeys.InternalJobId , jobDetails.Id.ToString() },
                    { TelemetryKeys.JobId, jobDetails.DcJobId.ToString() },
                    { TelemetryKeys.CollectionPeriod , jobDetails.CollectionPeriod.ToString() },
                    { TelemetryKeys.AcademicYear , jobDetails.AcademicYear.ToString() }
                },
                new Dictionary<string, double> { { "LearnerCount", jobDetails.LearnerCount ?? 0 } }
            );
        }
    }
}