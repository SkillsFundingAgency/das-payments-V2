using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
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

        public EarningsJobClient(IMessageSession messageSession, IPaymentLogger logger)
        {
            this.messageSession = messageSession ?? throw new ArgumentNullException(nameof(messageSession));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartJob(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages, DateTimeOffset startTime)
        {
            logger.LogVerbose($"Sending request to record start of earnings job. Job Id: {jobId}, Ukprn: {ukprn}");
            generatedMessages.ForEach(message => logger.LogVerbose($"Learner command event id: {message.MessageId}, name: {message.MessageName}"));
            var batchSize = 1000; //TODO: this should come from config
            var skip = 0;
            List<GeneratedMessage> batch;
            while ((batch = generatedMessages.Skip(skip).Take(1000).ToList()).Count > 0)
            {
                skip += batchSize;
                var providerEarningsEvent = new RecordEarningsJob
                {
                    StartTime = startTime,
                    JobId = jobId,
                    Ukprn = ukprn,
                    IlrSubmissionTime = ilrSubmissionTime,
                    CollectionYear = collectionYear,
                    CollectionPeriod = collectionPeriod,
                    GeneratedMessages = batch,
                    LearnerCount = generatedMessages.Count
                };
                await messageSession.Send(providerEarningsEvent).ConfigureAwait(false);
            }
            logger.LogDebug($"Sent request to record start of earnings job. Job Id: {jobId}, Ukprn: {ukprn}");
        }

        public async Task ProcessedJobMessage(long jobId, Guid messageId, string messageName, List<GeneratedMessage> generatedMessages)
        {
            logger.LogVerbose($"Sending request to record successful processing of event. Job Id: {jobId}, Event: id: {messageId} ");
            var itemProcessedEvent = new RecordJobMessageProcessingStatus
            {
                JobId = jobId,
                Id = messageId,
                MessageName = messageName,
                EndTime = DateTimeOffset.UtcNow,
                GeneratedMessages = generatedMessages ?? new List<GeneratedMessage>(),
                Succeeded = true
            };
            await messageSession.Send(itemProcessedEvent).ConfigureAwait(false);
            logger.LogDebug($"Sent request to record successful processing of event. Job Id: {jobId}, Event: id: {messageId} ");
        }

        //private readonly IPaymentLogger logger;
        //private readonly IJobsDataContext dataContext;
        //private readonly ITelemetry telemetry;

        //public EarningsJobClient(IPaymentLogger logger, IJobsDataContext dataContext, ITelemetry telemetry)
        //{
        //    this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        //    this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        //    this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        //}

        //public async Task StartJob(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages, DateTimeOffset startTime)
        //{
        //    logger.LogVerbose($"Sending request to record start of earnings job. Job Id: {jobId}, Ukprn: {ukprn}");
        //    generatedMessages.ForEach(message => logger.LogVerbose($"Learner command event id: {message.MessageId}, name: {message.MessageName}"));
        //    await SaveNewJob(jobId, ukprn, ilrSubmissionTime, collectionYear, collectionPeriod, generatedMessages, startTime).ConfigureAwait(false);
        //    logger.LogInfo($"Finished start job for job {jobId}, Ukprn: {ukprn}.");
        //}

        //private async Task SaveNewJob(long jobId, long ukprn, DateTime ilrSubmissionTime, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages, DateTimeOffset startTime)
        //{
        //    logger.LogDebug($"Now recording new provider earnings job.  Job Id: {jobId}, Ukprn: {ukprn}.");
        //    var jobDetails = new JobModel
        //    {
        //        JobType = JobType.EarningsJob,
        //        StartTime = startTime,
        //        CollectionPeriod = collectionPeriod,
        //        AcademicYear = collectionYear,
        //        Ukprn = ukprn,
        //        DcJobId = jobId,
        //        IlrSubmissionTime = ilrSubmissionTime,
        //        Status = JobStatus.InProgress,
        //        LearnerCount = generatedMessages.Count
        //    };
        //    var jobSteps = generatedMessages.Select(msg => new JobStepModel
        //    {
        //        MessageId = msg.MessageId,
        //        StartTime = msg.StartTime,
        //        MessageName = msg.MessageName,
        //        Status = JobStepStatus.Queued
        //    }).ToList();
        //    var stopwatch = Stopwatch.StartNew();
        //    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        await dataContext.SaveNewJob(jobDetails, jobSteps);
        //        scope.Complete();
        //    }
        //    SendTelemetry(jobId, ukprn, collectionYear, collectionPeriod, generatedMessages.Count, jobDetails, stopwatch);
        //    logger.LogInfo($"Finished saving the job to the db.  Job id: {jobDetails.Id}, DC Job Id: {jobId}, Ukprn: {ukprn}.");
        //}

        //private void SendTelemetry(long jobId, long ukprn, short collectionYear, byte collectionPeriod, int learnerCount, JobModel jobDetails, Stopwatch stopwatch)
        //{
        //    stopwatch.Stop();
        //    var properties = new Dictionary<string, string>
        //    {
        //        {"JobType", JobType.EarningsJob.ToString("G")},
        //        {"Ukprn", ukprn.ToString()},
        //        {"Id", jobDetails.Id.ToString()},
        //        {"ExternalJobId", jobId.ToString()},
        //        {"CollectionPeriod", collectionPeriod.ToString()},
        //        {"CollectionYear", collectionYear.ToString()}
        //    };
        //    var metrics = new Dictionary<string, double>
        //    {
        //        { TelemetryKeys.Duration, stopwatch.ElapsedMilliseconds },
        //        { TelemetryKeys.Count, learnerCount }
        //    };
        //    telemetry.TrackEvent("Saved New Earnings Job", properties, metrics);
        //}
    }
}