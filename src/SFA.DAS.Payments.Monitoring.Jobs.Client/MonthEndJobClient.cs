using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public interface IMonthEndJobClient
    {
        Task StartJob(long jobId, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages);
    }

    public class MonthEndJobClient : IMonthEndJobClient
    {
        private readonly IMessageSession messageSession;
        private readonly IPaymentLogger logger;

        public MonthEndJobClient(IEndpointInstanceFactory factory, IPaymentLogger logger)
        {

            messageSession = factory?.GetEndpointInstance().Result ?? throw new ArgumentNullException();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task StartJob(long jobId, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages)
        {
            logger.LogVerbose($"Monitoring disabled for period end job. Job id: {jobId}, collection: {collectionPeriod}-{collectionYear}");

            //logger.LogVerbose($"Sending request to record start of month end job. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
            //generatedMessages.ForEach(message => logger.LogVerbose($"Learner command event id: {message.MessageId}"));

            //var providerEarningsEvent = new RecordStartedProcessingMonthEndJob
            //{
            //    JobId = jobId,
            //    CollectionYear = collectionYear,
            //    CollectionPeriod = collectionPeriod,
            //    GeneratedMessages = generatedMessages
            //};
            //await messageSession.Send(providerEarningsEvent).ConfigureAwait(false);
            //logger.LogDebug($"Sent request to record start of month end job. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
        }
    }
}