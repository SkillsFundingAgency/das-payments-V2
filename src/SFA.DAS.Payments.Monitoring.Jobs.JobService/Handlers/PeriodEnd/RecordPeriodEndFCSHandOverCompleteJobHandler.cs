using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.PeriodEnd
{
    public class
        RecordPeriodEndFcsHandOverCompleteJobHandler : IHandleMessageBatches<RecordPeriodEndFcsHandOverCompleteJob>
    {
        private readonly IPeriodEndArchiveConfiguration archiveConfiguration;
        private readonly IJobStatusManager jobStatusManager;
        private readonly IPaymentLogger logger;
        private readonly IPeriodEndJobService periodEndJobService;


        public RecordPeriodEndFcsHandOverCompleteJobHandler(
            IPaymentLogger logger,
            IPeriodEndJobService periodEndJobService,
            IJobStatusManager jobStatusManager,
            IPeriodEndArchiveConfiguration archiveConfiguration)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.periodEndJobService =
                periodEndJobService ?? throw new ArgumentNullException(nameof(periodEndJobService));
            this.jobStatusManager =
                jobStatusManager ?? throw new ArgumentNullException(nameof(jobStatusManager));
            this.archiveConfiguration = archiveConfiguration;
        }

        public async Task Handle(IList<RecordPeriodEndFcsHandOverCompleteJob> messages,
            CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                logger.LogInfo($"Handling period end FCS handover job: {message.ToJson()}");
                await periodEndJobService.RecordPeriodEndJob(message, cancellationToken);
                await StartArchive(message);
                jobStatusManager.StartMonitoringJob(message.JobId, JobType.PeriodEndFcsHandOverCompleteJob);

                logger.LogInfo($"Handling period end FCS handover job: {message.JobId}");
            }
        }

        private async Task StartArchive(RecordPeriodEndFcsHandOverCompleteJob message)
        {
            try
            {
                logger.LogInfo(
                    $"Starting period end archive function. JobId: ${message.JobId}");
                var messageContent = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8,
                    "application/json");
                var client = new HttpClient();

                client.DefaultRequestHeaders.Add("x-functions-key", archiveConfiguration.ArchiveApiKey);

                var result = await client.PostAsync(
                    archiveConfiguration.ArchiveFunctionUrl, messageContent);

                if (!result.IsSuccessStatusCode)
                    logger.LogError(
                        $"{result.StatusCode}: HTTP error when starting period end archive function. Error: {result.Content}");
                logger.LogInfo(
                    $"Successfully called period end archive function. JobId: ${message.JobId}");
            }

            catch (Exception e)
            {
                logger.LogError(
                    $"Unable to start Period end archive function. Url: {archiveConfiguration.ArchiveFunctionUrl}. Timeout: {archiveConfiguration.ArchiveTimeout}",
                    e);
            }
        }
    }
}