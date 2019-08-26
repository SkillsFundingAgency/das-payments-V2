using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Exceptions;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface IJobMessageService
    {
        Task JobMessageCompleted(RecordJobMessageProcessingStatus jobMessageStatus, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class JobMessageService : IJobMessageService
    {
        private readonly IJobStorageService jobStorageService;
        private readonly IPaymentLogger logger;
        private readonly ITelemetry telemetry;

        public JobMessageService(IJobStorageService jobStorageService, IPaymentLogger logger, ITelemetry telemetry)
        {
            this.jobStorageService = jobStorageService ?? throw new ArgumentNullException(nameof(jobStorageService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task JobMessageCompleted(RecordJobMessageProcessingStatus jobMessageStatus, CancellationToken cancellationToken = default(CancellationToken))
        {
            logger.LogVerbose($"Now recording completion of message processing.  Job Id: {jobMessageStatus.JobId}, Message id: {jobMessageStatus.Id}.");

            var inProgressMessages = await jobStorageService.GetInProgressMessageIdentifiers(cancellationToken)
                .ConfigureAwait(false);

            if (inProgressMessages.Contains(jobMessageStatus.Id))
                inProgressMessages.Remove(jobMessageStatus.Id);

            foreach (var generatedMessage in jobMessageStatus.GeneratedMessages)
            {
                if (!await jobStorageService.StoredJobMessage(generatedMessage.MessageId, cancellationToken))
                    inProgressMessages.Add(generatedMessage.MessageId);
            }

            await jobStorageService.StoreInProgressMessageIdentifiers(inProgressMessages, cancellationToken)
                .ConfigureAwait(false);

            var jobStatus = await jobStorageService.GetJobStatus(cancellationToken).ConfigureAwait(false);
            if (!jobStatus.endTime.HasValue || jobStatus.endTime.Value < jobMessageStatus.EndTime)
                jobStatus.endTime = jobMessageStatus.EndTime;
            if (jobStatus.jobStatus != JobStepStatus.Failed)
                jobStatus.jobStatus = jobMessageStatus.Succeeded ? JobStepStatus.Completed : JobStepStatus.Failed;

            await jobStorageService.StoreJobStatus(jobStatus.jobStatus, jobStatus.endTime, cancellationToken).ConfigureAwait(false);

            var messageIds = new List<Guid> { jobMessageStatus.Id };
            messageIds.AddRange(jobMessageStatus.GeneratedMessages.Select(msg => msg.MessageId));

            var jobMessages = await jobStorageService.GetJobMessages(messageIds, cancellationToken).ConfigureAwait(false);
            var completedJobMessage = jobMessages.FirstOrDefault(msg => msg.MessageId == jobMessageStatus.Id);
            if (completedJobMessage == null)
            {
                completedJobMessage = new JobStepModel
                {
                    MessageId = jobMessageStatus.Id,
                    MessageName = jobMessageStatus.MessageName,
                };
                jobMessages.Add(completedJobMessage);
            }

            completedJobMessage.EndTime = jobMessageStatus.EndTime;
            completedJobMessage.Status = jobMessageStatus.Succeeded ? JobStepStatus.Completed : JobStepStatus.Failed;

            foreach (var generatedMessage in jobMessageStatus.GeneratedMessages)
            {
                var jobMessage = jobMessages.FirstOrDefault(msg => msg.MessageId == jobMessageStatus.Id);
                if (jobMessage == null)
                {
                    jobMessage = new JobStepModel
                    {
                        MessageId = jobMessageStatus.Id,
                        MessageName = jobMessageStatus.MessageName,
                    };
                    jobMessages.Add(jobMessage);
                }

                jobMessage.EndTime = jobMessageStatus.EndTime;
                jobMessage.Status = jobMessageStatus.Succeeded ? JobStepStatus.Completed : JobStepStatus.Failed;
            }
            await jobStorageService.StoreJobMessages(jobMessages, cancellationToken);

            logger.LogDebug("Finished saving updated job steps to db.");
            SendTelemetry(jobMessages);
            logger.LogInfo($"Recorded completion of message processing.  Job Id: {jobMessageStatus.JobId}, Message id: {jobMessageStatus.Id}.");
        }

        //private async Task<JobModel> GetJob(long dcJobId)
        //{
        //    var key = $"job_model_{dcJobId}";
        //    return await cache.GetOrCreateAsync<JobModel>(key, async ce =>
        //    {
        //        var jobModel = await dataContext.GetJobByDcJobId(dcJobId) ?? throw new DcJobNotFoundException(dcJobId);
        //        ce.Value = jobModel;
        //        ce.SlidingExpiration = TimeSpan.FromSeconds(120);
        //        return jobModel;
        //    });
        //}

        private void SendTelemetry(List<JobStepModel> jobMessages)
        {
            jobMessages
                .Where(msg => msg.EndTime != null && msg.StartTime != null)
                .ToList()
                .ForEach(jobMessage =>
            {
                logger.LogVerbose($"Now generating telemetry for completed message {jobMessage.MessageId}, {jobMessage.MessageName}");
                var props = new Dictionary<string, string>
                {
                    { TelemetryKeys.MessageName, jobMessage.MessageName },
                    { "JobId", jobMessage.JobId.ToString()},
                    { TelemetryKeys.Id, jobMessage.Id.ToString() },
                    { "MessageId",jobMessage.MessageId.ToString("N") },
                    { "MessageName",jobMessage.MessageName },
                    { "Status",jobMessage.Status.ToString("G") },
                    //{ TelemetryKeys.ExternalJobId, job.DcJobId.ToString() },
                    //{ TelemetryKeys.CollectionPeriod, job.CollectionPeriod.ToString() },
                    //{ TelemetryKeys.AcademicYear, job.AcademicYear.ToString()}
                };
                //                if (jobMessage. != null)
                //                    props.Add(TelemetryKeys.Ukprn, job.Ukprn.ToString());
                telemetry.TrackEvent("Processed Message", props, new Dictionary<string, double> { { TelemetryKeys.Duration, (jobMessage.EndTime.Value - jobMessage.StartTime.Value).TotalMilliseconds } });
            });
        }
    }
}