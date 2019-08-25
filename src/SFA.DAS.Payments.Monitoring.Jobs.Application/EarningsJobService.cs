using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface IEarningsJobService
    {
        Task JobStarted(RecordEarningsJob startedEvent, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class EarningsJobService : IEarningsJobService
    {
        private readonly IPaymentLogger logger;
        private readonly IJobStorageService jobStorageService;
        private readonly ITelemetry telemetry;

        public EarningsJobService(IPaymentLogger logger, IJobStorageService jobStorageService, ITelemetry telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.jobStorageService = jobStorageService ?? throw new ArgumentNullException(nameof(jobStorageService));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task JobStarted(RecordEarningsJob startedEvent, CancellationToken cancellationToken = default(CancellationToken))
        {
            logger.LogDebug($"Now recording new provider earnings job.  Job Id: {startedEvent.JobId}, Ukprn: {startedEvent.Ukprn}.");

            var jobDetails = new JobModel
            {
                JobType = JobType.EarningsJob,
                StartTime = startedEvent.StartTime,
                CollectionPeriod = startedEvent.CollectionPeriod,
                AcademicYear = startedEvent.CollectionYear,
                Ukprn = startedEvent.Ukprn,
                DcJobId = startedEvent.JobId,
                IlrSubmissionTime = startedEvent.IlrSubmissionTime,
                Status = JobStatus.InProgress,
                LearnerCount = startedEvent.LearnerCount
            };
            await jobStorageService.StoreNewJob(jobDetails, CancellationToken.None);
            logger.LogDebug($"Finished storing new job: {jobDetails.Id} for dc job id: {jobDetails.DcJobId}, Ukprn: {startedEvent.Ukprn}. Now storing the job messages.");


            var inProgressMessages = await jobStorageService.GetInProgressMessageIdentifiers(cancellationToken)
                .ConfigureAwait(false);

            foreach (var generatedMessage in startedEvent.GeneratedMessages)
            {
                if (!await jobStorageService.StoredJobMessage(generatedMessage.MessageId, cancellationToken))
                    inProgressMessages.Add(generatedMessage.MessageId);
            }

            await jobStorageService.StoreInProgressMessageIdentifiers(inProgressMessages, cancellationToken)
                .ConfigureAwait(false);

            await StoreJobMessages(jobDetails.Id, startedEvent.GeneratedMessages, cancellationToken);
            logger.LogDebug($"Finished storing new job messages for job: {jobDetails.Id} for dc job id: {jobDetails.DcJobId}, Ukprn: {startedEvent.Ukprn}.");
            SendTelemetry(startedEvent, jobDetails);
            logger.LogInfo($"Finished saving the job info.  Job id: {jobDetails.Id}, DC Job Id: {startedEvent.JobId}, Ukprn: {startedEvent.Ukprn}.");
        }

        private async Task StoreJobMessages(long internalJobId, List<GeneratedMessage> generatedMessages, CancellationToken cancellationToken)
        {
            var jobMessages = await jobStorageService.GetJobMessages(generatedMessages.Select(msg => msg.MessageId).ToList(), cancellationToken);
            generatedMessages.ForEach(generatedMessage =>
            {
                var jobMessage = jobMessages.FirstOrDefault(msg => msg.MessageId == generatedMessage.MessageId);
                if (jobMessage == null)
                {
                    jobMessage = new JobStepModel
                    {
                        MessageId = generatedMessage.MessageId,
                        Status = JobStepStatus.Queued,
                    };
                    jobMessages.Add(jobMessage);
                }
                jobMessage.StartTime = generatedMessage.StartTime;
                jobMessage.MessageName = generatedMessage.MessageName;
                jobMessage.JobId = internalJobId;
            });
            await jobStorageService.StoreJobMessages(jobMessages, cancellationToken);
        }

        //private async Task SaveNewJob(RecordEarningsJob startedEvent)
        //{
        //    logger.LogDebug($"Now recording new provider earnings job.  Job Id: {startedEvent.JobId}, Ukprn: {startedEvent.Ukprn}.");

        //    var jobDetails = new JobModel
        //    {
        //        JobType = JobType.EarningsJob,
        //        StartTime = startedEvent.StartTime,
        //        CollectionPeriod = startedEvent.CollectionPeriod,
        //        AcademicYear = startedEvent.CollectionYear,
        //        Ukprn = startedEvent.Ukprn,
        //        DcJobId = startedEvent.JobId,
        //        IlrSubmissionTime = startedEvent.IlrSubmissionTime,
        //        Status = JobStatus.InProgress,
        //        LearnerCount = startedEvent.LearnerCount
        //    };
        //    //var jobSteps = startedEvent.GeneratedMessages.Select(msg => new JobStepModel
        //    //{
        //    //    MessageId = msg.MessageId,
        //    //    StartTime = msg.StartTime,
        //    //    MessageName = msg.MessageName,
        //    //    Status = JobStepStatus.Queued,

        //    //}).ToList();
        //    //using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    //{
        //    //    await dataContext.SaveNewJob(jobDetails, jobSteps);
        //    //    scope.Complete();
        //    //}
        //    SendTelemetry(startedEvent, jobDetails);
        //    logger.LogInfo($"Finished saving the job to the db.  Job id: {jobDetails.Id}, DC Job Id: {startedEvent.JobId}, Ukprn: {startedEvent.Ukprn}.");
        //}

        //private async Task SaveJobSteps(RecordEarningsJob startedEvent, JobModel jobDetails)
        //{
        //    var jobStepIds = startedEvent.GeneratedMessages.Select(generatedMessage => generatedMessage.MessageId)
        //        .ToList();
        //    var jobSteps = await dataContext.GetJobSteps(jobStepIds).ConfigureAwait(false);
        //    startedEvent.GeneratedMessages.ForEach(generatedMessage =>
        //    {
        //        var jobStep = jobSteps.FirstOrDefault(step => step.MessageId == generatedMessage.MessageId);
        //        if (jobStep == null)
        //        {
        //            jobStep = new JobStepModel
        //            {
        //                JobId = jobDetails.Id,
        //                MessageId = generatedMessage.MessageId,
        //                MessageName = generatedMessage.MessageName,
        //                StartTime = generatedMessage.StartTime,
        //                Status = JobStepStatus.Queued
        //            };
        //            jobSteps.Add(jobStep);
        //        }

        //        jobStep.StartTime = generatedMessage.StartTime;
        //    });

        //    await dataContext.SaveJobSteps(jobSteps).ConfigureAwait(false);
        //    logger.LogInfo($"Finished saving the job steps to the db.  Job id: {jobDetails.Id}, DC Job Id: {startedEvent.JobId}, Ukprn: {startedEvent.Ukprn}.");

        //}

        private void SendTelemetry(RecordEarningsJob startedEvent, JobModel jobDetails)
        {
            //telemetry.AddProperty("JobType", JobType.EarningsJob.ToString("G"));
            //telemetry.AddProperty("Ukprn", startedEvent.Ukprn.ToString());
            //telemetry.AddProperty("Id", jobDetails.Id.ToString());
            //telemetry.AddProperty("ExternalJobId", startedEvent.JobId.ToString());
            //telemetry.AddProperty("CollectionPeriod", startedEvent.CollectionPeriod.ToString());
            //telemetry.AddProperty("CollectionYear", startedEvent.CollectionYear.ToString());
            //telemetry.TrackEvent("Started Job", startedEvent.LearnerCount);
        }
    }
}