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
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.PeriodEnd
{
    public class RecordPeriodEndRequestReportsJobHandler : IHandleMessageBatches<RecordPeriodEndRequestReportsJob>
    {
        private readonly IPaymentLogger logger;
        private readonly IPeriodEndJobService periodEndJobService;
        private readonly IPeriodEndReportValidationClient periodEndReportValidationClient;
        private readonly IJobStorageService jobStorageService;

        //private readonly ITelemetry telemetry;

        public RecordPeriodEndRequestReportsJobHandler(IPaymentLogger logger, IPeriodEndJobService periodEndJobService, IJobStorageService jobStorageService, IPeriodEndReportValidationClient periodEndReportValidationClient)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.periodEndJobService = periodEndJobService ?? throw new ArgumentNullException(nameof(periodEndJobService));
            this.jobStorageService = jobStorageService ?? throw new ArgumentNullException(nameof(jobStorageService));
            this.periodEndReportValidationClient = periodEndReportValidationClient ?? throw new ArgumentNullException(nameof(periodEndReportValidationClient));
        }

        public async Task Handle(IList<RecordPeriodEndRequestReportsJob> messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                logger.LogInfo($"Handling period end request reports job: {message.ToJson()}");

                await periodEndJobService.RecordPeriodEndJob(message, cancellationToken);

                var metricsValid = await periodEndReportValidationClient.RequestReports(message.JobId, message.CollectionYear, message.CollectionPeriod);

                var jobStatus = metricsValid ? JobStatus.Completed : JobStatus.CompletedWithErrors;

                await jobStorageService.SaveJobStatus(message.JobId, jobStatus, DateTimeOffset.Now, cancellationToken);

                logger.LogInfo($"Handled period end request reports job: {message.JobId}");

                stopwatch.Stop();

                //todo is Telemetry required?
                //SendTelemetry(message, jobStatus, stopwatch.Elapsed);
            }
        }
    }
}