using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface IJobMessageService
    {
        Task RecordCompletedJobMessageStatus(RecordJobMessageProcessingStatus jobMessageStatus, CancellationToken cancellationToken = default(CancellationToken));
        Task RecordStartedJobMessages(RecordStartedProcessingJobMessages message, CancellationToken cancellationToken);
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

        public async Task RecordCompletedJobMessageStatus(RecordJobMessageProcessingStatus jobMessageStatus, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (jobMessageStatus.GeneratedMessages.Any())
            {
                logger.LogVerbose($"Received branch level job status message, this message will be ignored will be ignored. Message id: {jobMessageStatus.Id}");
                return;
            }

            if (await IsDuplicate(jobMessageStatus.Id, cancellationToken).ConfigureAwait(false))
                return;

            var jobStatus = await jobStorageService.GetJobStatus(cancellationToken).ConfigureAwait(false);
            if (!jobStatus.endTime.HasValue || jobStatus.endTime.Value < jobMessageStatus.EndTime)
                jobStatus.endTime = jobMessageStatus.EndTime;
            if (jobStatus.jobStatus != JobStepStatus.Failed)
                jobStatus.jobStatus = jobMessageStatus.Succeeded ? JobStepStatus.Completed : JobStepStatus.Failed;

            await jobStorageService.StoreJobStatus(jobStatus.jobStatus, jobStatus.endTime, cancellationToken).ConfigureAwait(false);

            if (await jobStorageService.HasInProgressMessage(jobMessageStatus.Id, cancellationToken).ConfigureAwait(false))
                await jobStorageService
                    .RemoveInProgressMessage(jobMessageStatus.Id, cancellationToken)
                    .ConfigureAwait(false);
            else
                await jobStorageService
                    .AddCompletedMessage(jobMessageStatus.Id, cancellationToken)
                    .ConfigureAwait(false);
            logger.LogDebug($"Recorded completion of message processing.  Job Id: {jobMessageStatus.JobId}, Message id: {jobMessageStatus.Id}.");
        }

        private async Task<bool> IsDuplicate(Guid id, CancellationToken cancellationToken)
        {
            var history = await jobStorageService.GetCompletedMessageIdentifiersHistory(cancellationToken)
                .ConfigureAwait(false);
            if (history.Contains(id))
                return true;

            history.Add(id);
            await jobStorageService.StoreCompletedMessageIdentifiersHistory(history, cancellationToken);
            return false;
        }

        public async Task RecordStartedJobMessages(RecordStartedProcessingJobMessages message, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Recording started processing last messages in a job.");
            var completedMessages = await jobStorageService.GetCompletedMessageIdentifiers(cancellationToken);

            var inProgressMessages = message.GeneratedMessages
                .Where(startedMessage => !completedMessages.Contains(startedMessage.MessageId))
                .Select(msg => msg.MessageId)
                .ToList();

            await jobStorageService.AddInProgressMessages(inProgressMessages, cancellationToken).ConfigureAwait(false);

            var receivedCompletedMessages = message.GeneratedMessages
                .Where(startedMessage => completedMessages.Contains(startedMessage.MessageId))
                .Select(msg => msg.MessageId)
                .ToList();

            await jobStorageService.RemoveCompletedMessages(receivedCompletedMessages, cancellationToken)
                .ConfigureAwait(false);

            logger.LogDebug($"Recorded started processing last messages in a job.");
        }
    }
}