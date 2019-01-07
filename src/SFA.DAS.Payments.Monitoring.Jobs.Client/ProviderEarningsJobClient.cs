using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public interface IProviderEarningsJobClient
    {
        Task StartJob(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages);
        Task ProcessedJobMessage(long jobId, Guid messageId, string messageName, List<GeneratedMessage> generatedMessages);
    }

    public class ProviderEarningsJobClient : IProviderEarningsJobClient
    {
        private readonly IMessageSession messageSession;
        private readonly IPaymentLogger logger;

        public ProviderEarningsJobClient(IEndpointInstanceFactory factory, IPaymentLogger logger)
        {

            messageSession = factory?.GetEndpointInstance().Result ?? throw new ArgumentNullException();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartJob(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages)
        {
            logger.LogVerbose($"Sending request to record start of provider earnings job. Job Id: {jobId}, Ukprn: {ukprn}");
            generatedMessages.ForEach(message => logger.LogVerbose($"Learner command event id: {message.MessageId}"));
            var providerEarningsEvent = new RecordStartedProcessingProviderEarningsJob
            {
                JobId = jobId,
                Ukprn = ukprn,
                IlrSubmissionTime = ilrSubmissionTime,
                CollectionYear = collectionYear,
                CollectionPeriod = collectionPeriod,
                GeneratedMessages = generatedMessages
            };
            await messageSession.Send(providerEarningsEvent).ConfigureAwait(false);
            logger.LogDebug($"Sent request to record start of provider earnings job. Job Id: {jobId}, Ukprn: {ukprn}");
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
    }
}