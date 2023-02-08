using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobMessageProcessing
{
    public interface IJobMessageService
    {
        Task RecordCompletedJobMessageStatus(IList<RecordJobMessageProcessingStatus> jobMessages, CancellationToken cancellationToken);
    }

    public class JobMessageService : IJobMessageService
    {
        private readonly IJobStorageService jobStorageService;
        private readonly IPaymentLogger logger;

        public JobMessageService(IJobStorageService jobStorageService, IPaymentLogger logger)
        {
            this.jobStorageService = jobStorageService ?? throw new ArgumentNullException(nameof(jobStorageService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RecordCompletedJobMessageStatus(IList<RecordJobMessageProcessingStatus> jobMessages, CancellationToken cancellationToken)
        {
            var jobGroup = jobMessages.GroupBy(g => g.JobId).ToList();

            foreach (var jobMessage in jobGroup)
            {
                var first = jobMessage.First();

                var completedMessages = jobMessages.Select(statusMessage => new CompletedMessage
                {
                    MessageId = statusMessage.Id,
                    JobId = statusMessage.JobId,
                    CompletedTime = statusMessage.EndTime,
                    Succeeded = statusMessage.Succeeded
                }).ToList();

                logger.LogVerbose($"Now storing the completed messages. Job Id: {first.JobId}");

                await jobStorageService.StoreCompletedMessage(first.JobId, completedMessages, cancellationToken);

                var inProgressMessages = jobMessages.SelectMany(sm => sm.GeneratedMessages.Select(message => new InProgressMessage
                {
                    MessageId = message.MessageId,
                    JobId = first.JobId,
                    MessageName = message.MessageName
                })).ToList();

                logger.LogVerbose($"Stored completed message. Now storing {inProgressMessages.Count} in progress messages generated while processing message. job Id: {first.JobId}");

                await jobStorageService.StoreInProgressMessages(first.JobId, inProgressMessages, cancellationToken);

                logger.LogInfo($"Recorded completion of message processing. completed messages {completedMessages.Count}, in progress messages {inProgressMessages.Count}, Job Id: {first.JobId}.");
            }
        }
    }
}