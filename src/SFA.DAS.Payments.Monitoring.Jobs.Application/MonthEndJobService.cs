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
        Task JobStarted(RecordPeriodEndStartJob startedStartJobCommand);
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

        public async Task JobStarted(RecordPeriodEndStartJob startedStartJobCommand)
        {
            logger.LogVerbose($"Now recording new month end job.  Job Id: {startedStartJobCommand.JobId}, Collection period: {startedStartJobCommand.CollectionYear}-{startedStartJobCommand.CollectionPeriod}.");
            var jobDetails = new JobModel
            {
                StartTime = startedStartJobCommand.StartTime,
                Status = JobStatus.InProgress,
                JobType = JobType.PeriodEndStartJob,
                CollectionPeriod = startedStartJobCommand.CollectionPeriod,
                AcademicYear = startedStartJobCommand.CollectionYear,
                DcJobId = startedStartJobCommand.JobId,
            };
            var jobSteps = startedStartJobCommand.GeneratedMessages.Select(msg => new JobStepModel
            {
                MessageId = msg.MessageId,
                StartTime = msg.StartTime,
                MessageName = msg.MessageName,
                Status = JobStepStatus.Queued,
            }).ToList();
            await dataContext.SaveNewJob(jobDetails, jobSteps);
            telemetry.AddProperty("JobType", jobDetails.JobType.ToString("G"));
            telemetry.AddProperty("JobId", startedStartJobCommand.JobId.ToString());
            telemetry.AddProperty("CollectionPeriod", startedStartJobCommand.CollectionPeriod.ToString());
            telemetry.AddProperty("CollectionYear", startedStartJobCommand.CollectionYear.ToString());
            telemetry.TrackEvent("Started Job");
            logger.LogDebug($"Finished recording new month end job.  Job Id: {startedStartJobCommand.JobId}, Collection period: {startedStartJobCommand.CollectionYear}-{startedStartJobCommand.CollectionPeriod}.");
        }

    }
}