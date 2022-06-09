using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobMessageProcessing
{
    public interface IJobMessageService
    {
        Task RecordCompletedJobMessageStatus(IList<RecordJobMessageProcessingStatus> statusMessages, CancellationToken cancellationToken);
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

        public async Task RecordCompletedJobMessageStatus(IList<RecordJobMessageProcessingStatus> statusMessages, CancellationToken cancellationToken)
        {
            var groupedStatusMessages = statusMessages.GroupBy(st => st.JobId).ToList();

            foreach(var messages in  groupedStatusMessages)
            {
                var completedMessage = messages.First();

                logger.LogVerbose($"Now storing the completed message. Message id: {completedMessage.Id}, Job: {completedMessage.JobId}, End time: {completedMessage.EndTime}, Succeeded: {completedMessage.Succeeded}");

                await jobStorageService.StoreCompletedMessage(messages.Select(completedMessages => new CompletedMessage
                {
                    MessageId = completedMessages.Id, 
                    JobId = completedMessages.JobId, 
                    CompletedTime = completedMessages.EndTime, 
                    Succeeded = completedMessages.Succeeded
                }), cancellationToken);

                var generatedMessages = messages.SelectMany(s => s.GeneratedMessages).ToList();

                logger.LogVerbose($"Stored completed message. Now storing {generatedMessages.Count} in progress messages generated while processing message: {completedMessage.Id} for job: {completedMessage.JobId}");

                await jobStorageService.StoreInProgressMessages(completedMessage.JobId, generatedMessages.Select(message => new InProgressMessage
                    {
                        MessageId = message.MessageId,
                        JobId = completedMessage.JobId,
                        MessageName = message.MessageName
                    }).ToList(), cancellationToken);

                logger.LogDebug($"Recorded completion of message processing.  Job Id: {completedMessage.JobId}, Message id: {completedMessage.Id}.");
            }
        }
    }
}