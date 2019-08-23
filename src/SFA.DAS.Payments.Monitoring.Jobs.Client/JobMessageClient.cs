using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public interface IJobMessageClient
    {
        Task ProcessedJobMessage(long jobId, Guid messageId, string messageName, List<GeneratedMessage> generatedMessages);
        Task ProcessingFailedForJobMessage(byte[] failedMessageBody);
    }

    public class JobMessageClient : IJobMessageClient
    {
        private readonly IMessageSession messageSession;
        private readonly IPaymentLogger logger;

        public JobMessageClient(IMessageSession messageSession, IPaymentLogger logger)
        {
            this.messageSession = messageSession ?? throw new ArgumentNullException(nameof(messageSession));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task ProcessedJobMessage(long jobId, Guid messageId, string messageName, List<GeneratedMessage> generatedMessages)
        {
            logger.LogVerbose($"Sending request to record successful processing of event. Job Id: {jobId}, Event: id: {messageId} ");
            var itemProcessedEvent = new RecordJobMessageProcessingStatus
            {
                JobId = jobId,
                Id = messageId,
                MessageName = messageName,
                EndTime = DateTimeOffset.UtcNow,
                GeneratedMessages = generatedMessages ?? new List<GeneratedMessage>(),
                Succeeded = true
            };
            await messageSession.Send(itemProcessedEvent).ConfigureAwait(false);
            logger.LogDebug($"Sent request to record successful processing of event. Job Id: {jobId}, Event: id: {messageId} ");
        }

        public async Task ProcessingFailedForJobMessage(byte[] failedMessageBody)
        {
            try
            {
                var messageJson = Encoding.UTF8.GetString(failedMessageBody).Trim(Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble()).ToCharArray());
                var message = JObject.Parse(messageJson);
                var messageId = message.ContainsKey("EventId") ? (string)message["EventId"] : message.ContainsKey("CommandId") ? (string)message["CommandId"] : (string)null;
                if (messageId == null)
                {
                    logger.LogVerbose("No message id found on the message.");
                    return;
                }
                var job = message.ContainsKey("JobId") ? (string)message["JobId"] : (string)null;
                if (job == null)
                {
                    logger.LogVerbose("No job id found on the message.");
                    return;
                }

                if (!long.TryParse(job,out long jobId))
                {
                    logger.LogVerbose($"No job id found on the message {messageId}.");
                    return;
                }

                var itemProcessedEvent = new RecordJobMessageProcessingStatus
                {
                    JobId = jobId,
                    Id = Guid.Parse(messageId),
                    MessageName = string.Empty,
                    EndTime = DateTimeOffset.UtcNow,
                    GeneratedMessages = new List<GeneratedMessage>(),
                    Succeeded = false
                };
                await messageSession.Send(itemProcessedEvent).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Couldn't generate the job message failed message for monitoring.  {ex.Message}");
            }
        }
    }
}