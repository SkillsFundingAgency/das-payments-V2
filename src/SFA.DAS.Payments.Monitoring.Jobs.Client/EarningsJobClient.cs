using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public interface IEarningsJobClient
    {
        Task StartJob(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages, DateTimeOffset startTime);
        Task RecordJobFailure(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod);
        Task RecordJobSuccess(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod);
    }

    public class EarningsJobClient : IEarningsJobClient
    {

        private readonly IMessageSession messageSession;
        private readonly IPaymentLogger logger;
        private readonly IConfigurationHelper config;
        private readonly JobMonitorPartition partitionName = new JobMonitorPartition();

        public EarningsJobClient(IMessageSession messageSession, IPaymentLogger logger, IConfigurationHelper config)
        {
            this.messageSession = messageSession ?? throw new ArgumentNullException(nameof(messageSession));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.config = config;
        }

        public async Task StartJob(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages, DateTimeOffset startTime)
        {
            logger.LogVerbose($"Sending request to record start of earnings job. Job Id: {jobId}, Ukprn: {ukprn}");
            try
            {
                var batchSize = 1000; //TODO: this should come from config
                List<GeneratedMessage> batch;
                var providerEarningsEvent = new RecordEarningsJob
                {
                    StartTime = startTime,
                    JobId = jobId,
                    Ukprn = ukprn,
                    IlrSubmissionTime = ilrSubmissionTime,
                    CollectionYear = collectionYear,
                    CollectionPeriod = collectionPeriod,
                    GeneratedMessages = generatedMessages.Take(batchSize).ToList(),
                    LearnerCount = generatedMessages.Count
                };
                var partitionedEndpointName = GetMonitoringEndpointForJob(jobId, ukprn);
                logger.LogVerbose($"Endpoint for RecordEarningsJob for Job Id {jobId} is `{partitionedEndpointName}`");
                await messageSession.Send(partitionedEndpointName, providerEarningsEvent).ConfigureAwait(false);

                var skip = batchSize;

                while ((batch = generatedMessages.Skip(skip).Take(1000).ToList()).Count > 0)
                {
                    skip += batchSize;
                    var providerEarningsAdditionalMessages = new RecordJobAdditionalMessages
                    {
                        JobId = jobId,
                        GeneratedMessages = batch,
                    };
                    await messageSession.Send(partitionedEndpointName, providerEarningsAdditionalMessages).ConfigureAwait(false);
                }
                logger.LogDebug($"Sent request(s) to record start of earnings job. Job Id: {jobId}, Ukprn: {ukprn}");
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Failed to send the request to record the earnings job. Job: {jobId}, Error: {ex.Message}. {ex}");
                throw;
            }
        }

        public async Task RecordJobFailure(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod)
        {
            await RecordJobStatus<RecordEarningsJobFailed>(jobId, ukprn, ilrSubmissionTime, collectionYear, collectionPeriod)
                .ConfigureAwait(false);
        }

        private async Task RecordJobStatus<T>(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod) where T : RecordEarningsJobStatus, new()
        {
            logger.LogVerbose($"Sending record job failed request to monitoring service for job: {jobId}, ukprn: {ukprn}");
            try
            {
                RecordEarningsJobStatus recordJobStatus = new T
                {
                    JobId = jobId,
                    Ukprn = ukprn,
                    CollectionPeriod = collectionPeriod,
                    AcademicYear = collectionYear,
                    IlrSubmissionDateTime = ilrSubmissionTime
                };
                var partitionedEndpointName = GetMonitoringEndpointForJob(jobId, ukprn);
                await messageSession.Send(partitionedEndpointName, recordJobStatus);
                logger.LogDebug($"Sent record job status event for job: {jobId}, ukprn: {ukprn}. Status command: {typeof(T).Name}");
            }
            catch (Exception e)
            {
                logger.LogWarning($"Failed to send the request to record the failed job '{jobId}'. Error: {e.Message}. {e}");
                throw;
            }
        }

        public async Task RecordJobSuccess(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod)
        {
            await RecordJobStatus<RecordEarningsJobSucceeded>(jobId, ukprn, ilrSubmissionTime, collectionYear, collectionPeriod)
                .ConfigureAwait(false);
        }

        public string GetMonitoringEndpointForJob(long jobId, long ukprn)
        {
            var jobsEndpointName = config.GetSettingOrDefault("Monitoring_JobsService_EndpointName", "sfa-das-payments-monitoring-jobs");
            return $"{jobsEndpointName}{partitionName.PartitionNameForJob(jobId, ukprn)}";
        }
    }
}