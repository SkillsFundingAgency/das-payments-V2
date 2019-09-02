using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Newtonsoft.Json.Linq;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Client.Infrastructure;
using SFA.DAS.Payments.Monitoring.Jobs.JobsService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public interface IJobMessageClient
    {
        Task ProcessedCompletedJobMessage(long jobId, Guid messageId, string messageName, bool allowJobCompletion);
        Task ProcessingFailedForJobMessage(byte[] failedMessageBody);
        Task RecordStartedProcessingJobMessages(long jobId, List<GeneratedMessage> generatedMessages);
    }

    public class JobMessageClient : IJobMessageClient
    {
        private readonly IMessageSession messageSession;
        private readonly IPaymentLogger logger;
        private readonly IActorProxyFactory proxyFactory;
        public JobMessageClient(IMessageSession messageSession, IPaymentLogger logger, IActorProxyFactory proxyFactory)
        {
            this.messageSession = messageSession ?? throw new ArgumentNullException(nameof(messageSession));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
        }

        public async Task ProcessedCompletedJobMessage(long jobId, Guid messageId, string messageName, bool allowJobCompletion)
        {
            logger.LogVerbose($"Sending request to record successful processing of event. Job Id: {jobId}, Event: id: {messageId} ");
            var itemProcessedEvent = new RecordJobMessageProcessingStatus
            {
                JobId = jobId,
                Id = messageId,
                MessageName = messageName,
                EndTime = DateTimeOffset.UtcNow,
                GeneratedMessages = new List<GeneratedMessage>(),
                Succeeded = true,
                AllowJobCompletion = allowJobCompletion
            };
            try
            {
                var actorId = new ActorId(jobId.ToString());
                var actor = proxyFactory.CreateActorProxy<IJobsService>(new Uri(ServiceUris.JobsServiceUri), actorId);
                await actor.RecordJobMessageProcessingStatus(itemProcessedEvent, CancellationToken.None).ConfigureAwait(false);
                logger.LogDebug($"Sent request to record successful processing of event. Job Id: {jobId}, Event: id: {messageId} ");
            }
            catch (Exception e)
            {
                logger.LogWarning($"Failed to invoke monitoring actor using remoting when trying to record status of a job message.  Falling back to messaging notification for Job: {jobId}, Message: {messageName}, Id: {messageId}. Error: {e.Message}. {e}");
                await messageSession.Send(itemProcessedEvent).ConfigureAwait(false);
            }
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

                if (!long.TryParse(job, out long jobId))
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
                try
                {
                    var actorId = new ActorId(jobId.ToString());
                    var actor = proxyFactory.CreateActorProxy<IJobsService>(new Uri(ServiceUris.JobsServiceUri), actorId);
                    await actor.RecordJobMessageProcessingStatus(itemProcessedEvent, CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    logger.LogWarning($"Failed to invoke monitoring actor using remoting when trying to record status of a failed job message.  Falling back to messaging notification for Job: {jobId}, Id: {messageId}. Error: {e.Message}. {e}");
                    await messageSession.Send(itemProcessedEvent).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Couldn't generate the job message failed message for monitoring.  {ex.Message}");
            }
        }

        public async Task RecordStartedProcessingJobMessages(long jobId, List<GeneratedMessage> generatedMessages)
        {
            logger.LogVerbose($"Sending request to record started processing job messages. Job Id: {jobId}");
            var message = new RecordStartedProcessingJobMessages
            {
                JobId = jobId,
                GeneratedMessages = generatedMessages
            };
            try
            {
                var actorId = new ActorId(jobId.ToString());
                var actor = proxyFactory.CreateActorProxy<IJobsService>(new Uri(ServiceUris.JobsServiceUri), actorId);
                await actor.RecordJobMessageProcessingStartedStatus(message, CancellationToken.None).ConfigureAwait(false);
                logger.LogVerbose($"Sent request to record started processing job messages. Job Id: {jobId}");
            }
            catch (Exception e)
            {
                logger.LogWarning($"Failed to invoke monitoring actor using remoting when trying to record started processing job messages.  Falling back to messaging notification for Job: {jobId}. Error: {e.Message}. {e}");
                await messageSession.Send(message).ConfigureAwait(false);
            }
        }
    }
}