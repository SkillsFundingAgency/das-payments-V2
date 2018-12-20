using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.JobStatus.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Client
{
    public interface IProviderEarningsJobStatusClient
    {
        Task StartJob(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod, List<(DateTimeOffset StartTime, Guid MessageIds)> eventIds);
        Task ProcessedJobMessage(long jobId, Guid messageId, List<(DateTimeOffset StartTime, Guid EventId)> generatedEvents);
    }

    public class ProviderEarningsJobStatusClient : IProviderEarningsJobStatusClient
    {
        private readonly IMessageSession messageSession;
        private readonly IPaymentLogger logger;

        public ProviderEarningsJobStatusClient(IEndpointInstanceFactory factory, IPaymentLogger logger)
        {

            messageSession = factory?.GetEndpointInstance().Result ?? throw new ArgumentNullException();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartJob(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod, List<(DateTimeOffset StartTime, Guid MessageIds)> eventIds)
        {
            logger.LogVerbose($"Sending request to record start of provider earnings job. Job Id: {jobId}, Ukprn: {ukprn}");
            eventIds.ForEach(eventId => logger.LogVerbose($"Learner command event id: {eventId}"));
            var providerEarningsEvent = new RecordStartedProcessingProviderEarningsJob
            {
                JobId = jobId,
                Ukprn = ukprn,
                IlrSubmissionTime = ilrSubmissionTime,
                CollectionYear = collectionYear,
                CollectionPeriod = collectionPeriod,
                SubEventIds = eventIds
            };
            await messageSession.Send(providerEarningsEvent).ConfigureAwait(false);
            logger.LogDebug($"Sent request to record start of provider earnings job. Job Id: {jobId}, Ukprn: {ukprn}");
        }

        public async Task ProcessedJobMessage(long jobId, Guid messageId, List<(DateTimeOffset StartTime, Guid EventId)> generatedEvents)
        {
            logger.LogVerbose($"Sending request to record successful processing of event. Job Id: {jobId}, Event: id: {messageId} ");
            var itemProcessedEvent = new RecordJobMessageProcessingStatus
            {
                JobId = jobId,
                Id = messageId,
                EndTime = DateTimeOffset.UtcNow,
                GeneratedEvents = generatedEvents ?? new List<(DateTimeOffset StartTime, Guid EventId)>(),
                Succeeded = true
            };
            await messageSession.Send(itemProcessedEvent).ConfigureAwait(false);
            logger.LogDebug($"Sent request to record successful processing of event. Job Id: {jobId}, Event: id: {messageId} ");
        }
    }
}