using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public interface IJobMessageClient
    {
        Task ProcessedJobMessage(long jobId, Guid messageId, string messageName, List<GeneratedMessage> generatedMessages);
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
            logger.LogVerbose($"Monitoring disabled for job message. Job id: {jobId}, message id:{messageId}");

            //logger.LogVerbose($"Sending request to record successful processing of event. Job Id: {jobId}, Event: id: {messageId} ");
            //var itemProcessedEvent = new RecordJobMessageProcessingStatus
            //{
            //    JobId = jobId,
            //    Id = messageId,
            //    MessageName = messageName,
            //    EndTime = DateTimeOffset.UtcNow,
            //    GeneratedMessages = generatedMessages ?? new List<GeneratedMessage>(),
            //    Succeeded = true
            //};
            //await messageSession.Send(itemProcessedEvent).ConfigureAwait(false);
            //logger.LogDebug($"Sent request to record successful processing of event. Job Id: {jobId}, Event: id: {messageId} ");
        }
    }
}