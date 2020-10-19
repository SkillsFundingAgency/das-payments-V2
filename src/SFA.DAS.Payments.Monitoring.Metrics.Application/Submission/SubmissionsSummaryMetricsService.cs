using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Submission
{
    public interface ISubmissionsSummaryMetricsService
    {
        Task GenrateSubmissionsSummaryMetrics(long jobId, short academicYear, byte collectionPeriod, CancellationToken cancellationToken);
    }

    public class SubmissionsSummaryMetricsService : ISubmissionsSummaryMetricsService
    {
        private readonly IPaymentLogger logger;
        private readonly ISubmissionMetricsRepository submissionRepository;
        private readonly ITelemetry telemetry;

        public SubmissionsSummaryMetricsService(IPaymentLogger logger, ISubmissionMetricsRepository submissionMetricsRepository, ITelemetry telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.submissionRepository = submissionMetricsRepository ?? throw new ArgumentNullException(nameof(submissionMetricsRepository));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task GenrateSubmissionsSummaryMetrics(long jobId, short academicYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogDebug($"Building metrics for job: {jobId}, Academic year: {academicYear}, Collection period: {collectionPeriod}");

                var stopwatch = Stopwatch.StartNew();

                var metrics = await submissionRepository.GetSubmissionsSummaryMetrics(jobId, academicYear, collectionPeriod, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                var dataDuration = stopwatch.ElapsedMilliseconds;

                logger.LogDebug($"finished getting data from databases for job: {jobId}, Took: {dataDuration}ms.");

                await submissionRepository.SaveSubmissionsSummaryMetrics(metrics, cancellationToken);
                
                stopwatch.Stop();

                SendTelemetry(metrics, stopwatch.ElapsedMilliseconds);

                logger.LogInfo($"Finished building Submissions Summary Metrics for job: {jobId}, Academic year: {academicYear}, Collection period: {collectionPeriod}. Took: {stopwatch.ElapsedMilliseconds}ms");

                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                logger.LogWarning($"Error building the Submissions Summary metrics report for job: {jobId}, Error: {e}");
                throw;
            }
        }

        private void SendTelemetry(SubmissionsSummaryModel metrics, long reportGenerationDuration)
        {
            var properties = new Dictionary<string, string>
            {
                { TelemetryKeys.JobId, metrics.JobId.ToString()},
                { TelemetryKeys.CollectionPeriod, metrics.CollectionPeriod.ToString()},
                { TelemetryKeys.AcademicYear, metrics.AcademicYear.ToString()},
            };

            var stats = new Dictionary<string, double>
            {
                { "ReportGenerationDuration", reportGenerationDuration },

            };

            telemetry.TrackEvent("Finished Generating Submissions Summary Metrics", properties, stats);
        }
    }
}