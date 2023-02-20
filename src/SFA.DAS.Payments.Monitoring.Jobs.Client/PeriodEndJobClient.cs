using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Client.Infrastructure.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public interface IPeriodEndJobClient
    {
        Task StartPeriodEndJob<T>(T periodEndJob) where T : RecordPeriodEndJob;
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

        public async Task StartPeriodEndJob<T>(T periodEndJob) where T : RecordPeriodEndJob
        {
            var jobName = periodEndJob.GetType().Name;

            logger.LogDebug($"Sending request to record start of {jobName}. Job Id: {periodEndJob.JobId}, collection period: {periodEndJob.CollectionYear}-{periodEndJob.CollectionPeriod}");

            var partitionedEndpointName = config.GetMonitoringEndpointName(periodEndJob.JobId);

            logger.LogVerbose($"Endpoint for PeriodEndJobClient for {jobName} with Job Id {periodEndJob.JobId} is `{partitionedEndpointName}`");

            await messageSession.Send(partitionedEndpointName, periodEndJob);

            logger.LogInfo($"Sent request to record period end job: {jobName}. Job Id: {periodEndJob.JobId}, collection period: {periodEndJob.CollectionYear}-{periodEndJob.CollectionPeriod}");
        }
    }
}