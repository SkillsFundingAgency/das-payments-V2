using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
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
    }

    public class PeriodEndJobClient : IPeriodEndJobClient
    {
        private readonly IMessageSession messageSession;
        private readonly IPaymentLogger logger;

        public PeriodEndJobClient(IMonitoringMessageSessionFactory factory, IPaymentLogger logger)
        {

            messageSession = factory?.Create() ?? throw new ArgumentNullException();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            await messageSession.Send(job).ConfigureAwait(false);
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
    }
}