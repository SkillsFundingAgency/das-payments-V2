using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.PeriodEnd
{
    public class RecordPeriodEndRequestReportsJobHandler : IHandleMessageBatches<RecordPeriodEndRequestReportsJob>
    {
        private readonly IPaymentLogger logger;
        private readonly IPeriodEndJobService periodEndJobService;
        private readonly IRequestReportsClient requestReportsClient;
        private readonly IJobStorageService jobStorageService;

        //private readonly ITelemetry telemetry;

        public RecordPeriodEndRequestReportsJobHandler(IPaymentLogger logger, IPeriodEndJobService periodEndJobService, IJobStorageService jobStorageService, IRequestReportsClient requestReportsClient)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.periodEndJobService = periodEndJobService ?? throw new ArgumentNullException(nameof(periodEndJobService));
            this.jobStorageService = jobStorageService ?? throw new ArgumentNullException(nameof(jobStorageService));
            this.requestReportsClient = requestReportsClient ?? throw new ArgumentNullException(nameof(requestReportsClient));
        }

        public async Task Handle(IList<RecordPeriodEndRequestReportsJob> messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                logger.LogInfo($"Handling period end submission window validation job: {message.ToJson()}");

                await periodEndJobService.RecordPeriodEndJob(message, cancellationToken);

                var metricsValid = await requestReportsClient.RequestReports(message.JobId, message.CollectionYear, message.CollectionPeriod);
                //todo call client

                //todo set job status based on result
                //var jobStatus = metricsValid ? JobStatus.Completed : JobStatus.CompletedWithErrors;

                //await jobStorageService.SaveJobStatus(message.JobId, jobStatus, DateTimeOffset.Now, cancellationToken);

                logger.LogInfo($"Handled period end submission window validation job: {message.JobId}");

                stopwatch.Stop();

                //todo is Telemetry required?
                //SendTelemetry(message, jobStatus, stopwatch.Elapsed);
            }
        }
    }
}