using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Client.Infrastructure.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public interface IPeriodEndJobClient
    {
        Task RecordPeriodEndStart(long jobId, short collectionYear, byte collectionPeriod,
            List<GeneratedMessage> generatedMessages);
        Task RecordPeriodEndRun(long jobId, short collectionYear, byte collectionPeriod,
            List<GeneratedMessage> generatedMessages);
        Task RecordPeriodEndStop(long jobId, short collectionYear, byte collectionPeriod,
            List<GeneratedMessage> generatedMessages);
        Task RecordPeriodEndSubmissionWindowValidation(long jobId, short collectionYear, byte collectionPeriod,
            List<GeneratedMessage> generatedMessages);
    }

    public class PeriodEndJobClient : IPeriodEndJobClient
    {
        private readonly IMessageSession messageSession;
        private readonly IPaymentLogger logger;
        private readonly IConfigurationHelper config;

        public PeriodEndJobClient(IMonitoringMessageSessionFactory factory, IPaymentLogger logger, IConfigurationHelper config)
        {

            messageSession = factory?.Create() ?? throw new ArgumentNullException();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }
        protected async Task StartJob<T>(long jobId, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages) where T: RecordPeriodEndJob, new()
        {
            logger.LogDebug($"Sending request to record start of period end job. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
            var job = new T
            {
                JobId = jobId,
                CollectionYear = collectionYear,
                CollectionPeriod = collectionPeriod,
                GeneratedMessages = generatedMessages
            };
            var partitionedEndpointName = config.GetMonitoringEndpointName(jobId);
            logger.LogVerbose($"Endpoint for PeriodEndJobClient for Job Id {jobId} is `{partitionedEndpointName}`");
            await messageSession.Send(partitionedEndpointName, job).ConfigureAwait(false);
            logger.LogInfo($"Sent request to record period end job. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
        }

        public async Task RecordPeriodEndStart(long jobId, short collectionYear, byte collectionPeriod,
            List<GeneratedMessage> generatedMessages)
        {
            logger.LogDebug($"Sending request to record period end start. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
            await StartJob<RecordPeriodEndStartJob>(jobId, collectionYear, collectionPeriod, generatedMessages)
                .ConfigureAwait(false);
            logger.LogInfo($"Sent request to record period end start job. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
        }

        public async Task RecordPeriodEndRun(long jobId, short collectionYear, byte collectionPeriod,
            List<GeneratedMessage> generatedMessages)
        {
            logger.LogDebug($"Sending request to record period end run. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
            await StartJob<RecordPeriodEndRunJob>(jobId, collectionYear, collectionPeriod, generatedMessages)
                .ConfigureAwait(false);
            logger.LogInfo($"Sent request to record period end run job. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
        }

        public async Task RecordPeriodEndStop(long jobId, short collectionYear, byte collectionPeriod,
            List<GeneratedMessage> generatedMessages)
        {
            logger.LogDebug($"Sending request to record period end stop. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
            await StartJob<RecordPeriodEndStopJob>(jobId, collectionYear, collectionPeriod, generatedMessages)
                .ConfigureAwait(false);
            logger.LogInfo($"Sent request to record period end stop job. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
        }

        public async Task RecordPeriodEndSubmissionWindowValidation(long jobId, short collectionYear, byte collectionPeriod,
            List<GeneratedMessage> generatedMessages)
        {
            logger.LogDebug($"Sending request to record period end submission window validation. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
            await StartJob<RecordPeriodEndSubmissionWindowValidationJob>(jobId, collectionYear, collectionPeriod, generatedMessages)
                .ConfigureAwait(false);
            logger.LogInfo($"Sent request to record period end submission window validation. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
        }
    }
}