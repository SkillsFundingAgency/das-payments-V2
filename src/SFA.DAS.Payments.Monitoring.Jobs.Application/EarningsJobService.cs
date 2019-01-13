using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Model;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{


    public interface IEarningsJobService
    {
        Task JobStarted(RecordStartedProcessingEarningsJob startedEvent);
    }

    public class EarningsJobService : IEarningsJobService
    {
        private readonly IPaymentLogger logger;
        private readonly IJobsDataContext dataContext;
        private readonly ITelemetry telemetry;

        public EarningsJobService(IPaymentLogger logger, IJobsDataContext dataContext, ITelemetry telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.telemetry = telemetry;
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task JobStarted(RecordStartedProcessingEarningsJob startedEvent)
        {
            logger.LogDebug($"Now recording new provider earnings job.  Job Id: {startedEvent.JobId}, Ukprn: {startedEvent.Ukprn}.");
            var jobDetails = new JobModel
            {
                JobType = JobType.EarningsJob,
                StartTime = startedEvent.StartTime,
                CollectionPeriod = startedEvent.CollectionPeriod,
                CollectionYear = startedEvent.CollectionYear,
                Ukprn = startedEvent.Ukprn,
                DcJobId = startedEvent.JobId,
                IlrSubmissionTime = startedEvent.IlrSubmissionTime,
                Status = JobStatus.InProgress,
                LearnerCount = startedEvent.GeneratedMessages.Count
            };
            var jobSteps = startedEvent.GeneratedMessages.Select(msg => new JobStepModel
            {
                MessageId = msg.MessageId,
                StartTime = msg.StartTime,
                MessageName = msg.MessageName,
                Status = JobStepStatus.Queued,

            }).ToList();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await dataContext.SaveNewJob(jobDetails, jobSteps);
                scope.Complete();
            }
            telemetry.AddProperty("JobType", JobType.EarningsJob.ToString("G"));
            telemetry.AddProperty("Ukprn", startedEvent.Ukprn.ToString());
            telemetry.AddProperty("Id", jobDetails.Id.ToString());
            telemetry.AddProperty("ExternalJobId", startedEvent.JobId.ToString());
            telemetry.AddProperty("CollectionPeriod", startedEvent.CollectionPeriod.ToString());
            telemetry.AddProperty("CollectionYear", startedEvent.CollectionYear.ToString());
            telemetry.TrackEvent("Started Job", (double)jobDetails.LearnerCount);
            logger.LogInfo($"Finished saving the job to the db.  Job id: {jobDetails.Id}, DC Job Id: {startedEvent.JobId}, Ukprn: {startedEvent.Ukprn}.");
        }

    }
}