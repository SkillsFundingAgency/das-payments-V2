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
    public class RecordPeriodEndSubmissionWindowValidationJobHandler : IHandleMessageBatches<RecordPeriodEndSubmissionWindowValidationJob>
    {
        private readonly IPaymentLogger logger;
        private readonly IPeriodEndJobService periodEndJobService;
        private readonly ISubmissionWindowValidationClient submissionWindowValidationClient;
        private readonly IJobStorageService jobStorageService;

        private readonly ITelemetry telemetry;

        public RecordPeriodEndSubmissionWindowValidationJobHandler(IPaymentLogger logger, IPeriodEndJobService periodEndJobService, ISubmissionWindowValidationClient submissionWindowValidationClient, IJobStorageService jobStorageService, ITelemetry telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.periodEndJobService = periodEndJobService ?? throw new ArgumentNullException(nameof(periodEndJobService));
            this.submissionWindowValidationClient = submissionWindowValidationClient ?? throw new ArgumentNullException(nameof(submissionWindowValidationClient));
            this.jobStorageService = jobStorageService ?? throw new ArgumentNullException(nameof(jobStorageService));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task Handle(IList<RecordPeriodEndSubmissionWindowValidationJob> messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                logger.LogInfo($"Handling period end submission window validation job: {message.ToJson()}");
                
                await periodEndJobService.RecordPeriodEndJob(message, cancellationToken);
                
                var metricsValid = await submissionWindowValidationClient.Validate(message.JobId, message.CollectionYear, message.CollectionPeriod);

                var jobStatus = metricsValid ? JobStatus.Completed : JobStatus.CompletedWithErrors;

                await jobStorageService.SaveJobStatus(message.JobId, jobStatus, DateTimeOffset.Now, cancellationToken);
                
                logger.LogInfo($"Handled period end submission window validation job: {message.JobId}");

                stopwatch.Stop();
                
                SendTelemetry(message, jobStatus, stopwatch.Elapsed);
            }
        }

        private void SendTelemetry(RecordPeriodEndSubmissionWindowValidationJob job, JobStatus jobStatus, TimeSpan elapsedTime)
        {
            var properties = new Dictionary<string, string>
            {
                { TelemetryKeys.JobId, job.JobId.ToString()},
                { TelemetryKeys.JobType, nameof(RecordPeriodEndSubmissionWindowValidationJob)},
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