using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public interface IEarningsJobClient
    {
        Task StartJob(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages, DateTimeOffset startTime);
    }

    public class EarningsJobClient : IEarningsJobClient
    {

        private readonly IMessageSession messageSession;
        private readonly IPaymentLogger logger;
        private readonly IConfigurationHelper config;

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

                var jobsEndpointName = config.GetSettingOrDefault("Monitoring_JobsService_EndpointName", "sfa-das-payments-monitoring-jobs");
                var partitionedEndpointName = $"{jobsEndpointName}{jobId % 20}";
                logger.LogVerbose($"Endpoint for RecordEarningsJob for Job Id {jobId} is `{partitionedEndpointName}`");
                await messageSession.Send(partitionedEndpointName, providerEarningsEvent).ConfigureAwait(false);

                var skip = batchSize;

                while ((batch = generatedMessages.Skip(skip).Take(1000).ToList()).Count > 0)
                {
                    skip += batchSize;
                    var providerEarningsAdditionalMessages = new RecordEarningsJobAdditionalMessages
                    {
                        JobId = jobId,
                        GeneratedMessages = batch,
                    };
                    await messageSession.Send(providerEarningsAdditionalMessages).ConfigureAwait(false);
                }
                logger.LogDebug($"Sent request(s) to record start of earnings job. Job Id: {jobId}, Ukprn: {ukprn}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to send the request to record the earnings job. Job: {jobId}, Error: {ex.Message}", ex);
                throw;
            }
        }
    }
}