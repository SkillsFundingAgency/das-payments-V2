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
    public interface ICommonJobService
    {
        Task RecordNewJobAdditionalMessages(RecordJobAdditionalMessages jobRequest, CancellationToken cancellationToken);
    }

    public class JobService : ICommonJobService
    {
        protected IPaymentLogger Logger { get;  }
        protected IJobStorageService JobStorageService { get;  }
        protected ITelemetry Telemetry { get;  }

        public JobService(IPaymentLogger logger, IJobStorageService jobStorageService, ITelemetry telemetry)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.JobStorageService = jobStorageService ?? throw new ArgumentNullException(nameof(jobStorageService));
            this.Telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        protected virtual async Task RecordNewJob(JobModel jobDetails, List<GeneratedMessage> generatedMessages, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!jobDetails.DcJobId.HasValue)
                throw new InvalidOperationException("No DcJob Id found on the job.");

            var isNewJob = await JobStorageService.StoreNewJob(jobDetails, CancellationToken.None);
            Logger.LogDebug($"Finished storing new job: {jobDetails.Id} for dc job id: {jobDetails.DcJobId}. Now storing the job messages.");
            await RecordJobInProgressMessages(jobDetails.DcJobId.Value, generatedMessages, cancellationToken).ConfigureAwait(false);
            Logger.LogDebug($"Finished storing new job messages for job: {jobDetails.Id} for dc job id: {jobDetails.DcJobId}.");
            if (isNewJob)
                SendTelemetry(jobDetails);
            Logger.LogInfo($"Finished saving the job info.  DC Job Id: {jobDetails.DcJobId}");
        }


        public async Task RecordNewJobAdditionalMessages(RecordJobAdditionalMessages jobRequest, CancellationToken cancellationToken)
        {
            await RecordJobInProgressMessages(jobRequest.JobId, jobRequest.GeneratedMessages, cancellationToken).ConfigureAwait(false);
            Logger.LogInfo($"Finished storing new job messages for job: {jobRequest.JobId}");
        }

        protected async Task RecordJobInProgressMessages(long jobId, List<GeneratedMessage> generatedMessages, CancellationToken cancellationToken)
        {
            await JobStorageService.StoreInProgressMessages(jobId,
                generatedMessages.Select(message => new InProgressMessage
                {
                    MessageId = message.MessageId,
                    JobId = jobId,
                    MessageName = message.MessageName
                }).ToList(), cancellationToken);
        }


        private void SendTelemetry(JobModel jobDetails)
        {
            Telemetry.TrackEvent("Started Job",
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