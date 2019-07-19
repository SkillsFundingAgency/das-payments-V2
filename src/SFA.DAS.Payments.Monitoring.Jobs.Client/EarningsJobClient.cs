using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public interface IEarningsJobClient
    {
        Task StartJob(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages, DateTimeOffset startTime);
        Task ProcessedJobMessage(long jobId, Guid messageId, string messageName, List<GeneratedMessage> generatedMessages);
    }

    public class EarningsJobClient : IEarningsJobClient
    {
        private readonly IMessageSession messageSession;
        private readonly IPaymentLogger logger;

        public EarningsJobClient(IMessageSession messageSession, IPaymentLogger logger)
        {
            this.messageSession = messageSession ?? throw new ArgumentNullException(nameof(messageSession));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartJob(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages, DateTimeOffset startTime)
        {
            logger.LogVerbose($"Sending request to record start of earnings job. Job Id: {jobId}, Ukprn: {ukprn}");
            generatedMessages.ForEach(message => logger.LogVerbose($"Learner command event id: {message.MessageId}, name: {message.MessageName}"));
            var batchSize = 1000; //TODO: this should come from config
            var skip = 0;
            List<GeneratedMessage> batch;
            while ((batch = generatedMessages.Skip(skip).Take(1000).ToList()).Count > 0)
            {
                skip += batchSize;
                var providerEarningsEvent = new RecordStartedProcessingEarningsJob
                {
                    StartTime = startTime,
                    JobId = jobId,
                    Ukprn = ukprn,
                    IlrSubmissionTime = ilrSubmissionTime,
                    CollectionYear = collectionYear,
                    CollectionPeriod = collectionPeriod,
                    GeneratedMessages = batch,
                    LearnerCount = generatedMessages.Count
                };
                await messageSession.Send(providerEarningsEvent).ConfigureAwait(false);
            }
            logger.LogDebug($"Sent request to record start of earnings job. Job Id: {jobId}, Ukprn: {ukprn}");
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