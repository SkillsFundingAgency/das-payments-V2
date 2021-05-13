using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
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
        private readonly IPeriodEndRequestReportClient periodEndRequestReportClient;
        private readonly IJobStorageService jobStorageService;

        private readonly ITelemetry telemetry;

        public RecordPeriodEndRequestReportsJobHandler(IPaymentLogger logger, IPeriodEndJobService periodEndJobService, IJobStorageService jobStorageService, IPeriodEndRequestReportClient periodEndRequestReportClient, ITelemetry telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.periodEndJobService = periodEndJobService ?? throw new ArgumentNullException(nameof(periodEndJobService));
            this.jobStorageService = jobStorageService ?? throw new ArgumentNullException(nameof(jobStorageService));
            this.periodEndRequestReportClient = periodEndRequestReportClient ?? throw new ArgumentNullException(nameof(periodEndRequestReportClient));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task Handle(IList<RecordPeriodEndRequestReportsJob> messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                logger.LogInfo($"Handling period end request reports job: {message.ToJson()}");

                await periodEndJobService.RecordPeriodEndJob(message, cancellationToken);

                var metricsValid = await periodEndRequestReportClient.RequestReports(message.JobId, message.CollectionYear, message.CollectionPeriod);

                var jobStatus = metricsValid ? JobStatus.Completed : JobStatus.CompletedWithErrors;

                await jobStorageService.SaveJobStatus(message.JobId, jobStatus, DateTimeOffset.Now, cancellationToken);

                logger.LogInfo($"Handled period end request reports job: {message.JobId}");

                stopwatch.Stop();

                SendTelemetry(message, jobStatus, stopwatch.Elapsed);
            }
        }

        private void SendTelemetry(RecordPeriodEndRequestReportsJob job, JobStatus jobStatus, TimeSpan elapsedTime)
        {
            var properties = new Dictionary<string, string>
            {
                { TelemetryKeys.JobId, job.JobId.ToString()},
                { TelemetryKeys.JobType, nameof(RecordPeriodEndRequestReportsJob)},
                { TelemetryKeys.CollectionPeriod, job.CollectionPeriod.ToString()},
                { TelemetryKeys.AcademicYear, job.CollectionYear.ToString()},
                { TelemetryKeys.Status, jobStatus.ToString("G")}
            };

            var metrics = new Dictionary<string, double>
            {
                { TelemetryKeys.Duration, elapsedTime.TotalMilliseconds},
            };

            telemetry.TrackEvent("Finished Job", properties, metrics);
        }
    }
}