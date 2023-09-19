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
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.PeriodEnd
{
    public class
        RecordPeriodEndFcsHandOverCompleteJobHandler : IHandleMessageBatches<RecordPeriodEndFcsHandOverCompleteJob>
    {
        private readonly IGenericPeriodEndJobStatusManager jobStatusManager;
        private readonly IPaymentLogger logger;
        private readonly IPeriodEndJobService periodEndJobService;
        private readonly IPeriodEndArchiverClient periodEndArchiverClient;
        private readonly IJobStorageService jobStorageService;
        private readonly ITelemetry telemetry;


        public RecordPeriodEndFcsHandOverCompleteJobHandler(
            IPaymentLogger logger,
            IPeriodEndJobService periodEndJobService,
            IGenericPeriodEndJobStatusManager jobStatusManager,
            IPeriodEndArchiverClient periodEndArchiverClient,
            IJobStorageService jobStorageService,
            ITelemetry telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.periodEndJobService =
                periodEndJobService ?? throw new ArgumentNullException(nameof(periodEndJobService));
            this.jobStatusManager = jobStatusManager ?? throw new ArgumentNullException(nameof(jobStatusManager));
            this.jobStorageService = jobStorageService ?? throw new ArgumentNullException(nameof(jobStorageService));
            this.periodEndArchiverClient = periodEndArchiverClient;
            this.periodEndArchiverClient = periodEndArchiverClient;
            this.jobStorageService = jobStorageService;
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task Handle(IList<RecordPeriodEndFcsHandOverCompleteJob> messages,
            CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                logger.LogInfo($"Handling period end Fcs handover job: {message.ToJson()}");
                await periodEndJobService.RecordPeriodEndJob(message, cancellationToken);
                jobStatusManager.StartMonitoringJob(message.JobId, JobType.PeriodEndFcsHandOverCompleteJob);

                await periodEndArchiverClient.StartArchive();

                // Poll Archive Status 
                var jobStatus = JobStatus.InProgress;

                while (jobStatus == JobStatus.InProgress)
                {
                    var response = await periodEndArchiverClient.ArchiveStatus();

                    jobStatus = response switch
                    {
                        "Started" => JobStatus.InProgress,
                        "InProgress" => JobStatus.InProgress,
                        "Queued" => JobStatus.InProgress,
                        "Failed" => JobStatus.DcTasksFailed,
                        "Succeeded" => JobStatus.Completed,
                        _ => jobStatus
                    };

                    await jobStorageService.SaveJobStatus(message.JobId, jobStatus, DateTimeOffset.Now, cancellationToken);
                    Thread.Sleep(TimeSpan.FromMinutes(10));
                }
                stopwatch.Stop();

                logger.LogInfo($"Handling period end Fcs handover job: {message.JobId}");
                SendTelemetry(message, jobStatus, stopwatch.Elapsed);

            }
        }

        private void SendTelemetry(RecordPeriodEndFcsHandOverCompleteJob job, JobStatus jobStatus, TimeSpan elapsedTime)
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