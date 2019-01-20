using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Model;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface IMonthEndJobService
    {
        Task JobStarted(RecordStartedProcessingMonthEndJob startedJobCommand);
    }

    public class MonthEndJobService : IMonthEndJobService
    {
        private readonly IPaymentLogger logger;
        private readonly IJobsDataContext dataContext;
        private readonly ITelemetry telemetry;

        public MonthEndJobService(IPaymentLogger logger, IJobsDataContext dataContext, ITelemetry telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.telemetry = telemetry;
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task JobStarted(RecordStartedProcessingMonthEndJob startedJobCommand)
        {
            logger.LogVerbose($"Now recording new month end job.  Job Id: {startedJobCommand.JobId}, Collection period: {startedJobCommand.CollectionYear}-{startedJobCommand.CollectionPeriod}.");
            var jobDetails = new JobModel
            {
                StartTime = startedJobCommand.StartTime,
                Status = JobStatus.InProgress,
                JobType = JobType.MonthEndJob,
                CollectionPeriod = startedJobCommand.CollectionPeriod,
                AcademicYear = startedJobCommand.CollectionYear,
                DcJobId = startedJobCommand.JobId,
            };
            var jobSteps = startedJobCommand.GeneratedMessages.Select(msg => new JobStepModel
            {
                MessageId = msg.MessageId,
                StartTime = msg.StartTime,
                MessageName = msg.MessageName,
                Status = JobStepStatus.Queued,
            }).ToList();
            await dataContext.SaveNewJob(jobDetails, jobSteps);
            telemetry.AddProperty("JobType", jobDetails.JobType.ToString("G"));
            telemetry.AddProperty("JobId", startedJobCommand.JobId.ToString());
            telemetry.AddProperty("CollectionPeriod", startedJobCommand.CollectionPeriod.ToString());
            telemetry.AddProperty("CollectionYear", startedJobCommand.CollectionYear.ToString());
            telemetry.TrackEvent("Started Job");
            logger.LogDebug($"Finished recording new month end job.  Job Id: {startedJobCommand.JobId}, Collection period: {startedJobCommand.CollectionYear}-{startedJobCommand.CollectionPeriod}.");
        }

    }
}