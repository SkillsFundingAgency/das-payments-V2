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
        Task RecordPeriodEndJob(RecordPeriodEndJob startedStartJobCommand);
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

        public async Task RecordPeriodEndJob(RecordPeriodEndJob periodEndJob)
        {
            logger.LogVerbose($"Now recording new month end job.  Job Id: {periodEndJob.JobId}, Collection period: {periodEndJob.CollectionYear}-{periodEndJob.CollectionPeriod}.");
            var jobDetails = new JobModel
            {
                StartTime = periodEndJob.StartTime,
                Status = JobStatus.InProgress,
                JobType = ToJobType(periodEndJob),
                CollectionPeriod = periodEndJob.CollectionPeriod,
                AcademicYear = periodEndJob.CollectionYear,
                DcJobId = periodEndJob.JobId,
            };
            var jobSteps = periodEndJob.GeneratedMessages.Select(msg => new JobStepModel
            {
                MessageId = msg.MessageId,
                StartTime = msg.StartTime,
                MessageName = msg.MessageName,
                Status = JobStepStatus.Queued,
            }).ToList();
            await dataContext.SaveNewJob(jobDetails, jobSteps);
            telemetry.AddProperty("JobType", jobDetails.JobType.ToString("G"));
            telemetry.AddProperty("JobId", periodEndJob.JobId.ToString());
            telemetry.AddProperty("CollectionPeriod", periodEndJob.CollectionPeriod.ToString());
            telemetry.AddProperty("CollectionYear", periodEndJob.CollectionYear.ToString());
            telemetry.TrackEvent("Started Job");
            logger.LogDebug($"Finished recording new month end job.  Job Id: {periodEndJob.JobId}, Job type: {jobDetails.JobType:G}, Collection period: {periodEndJob.CollectionYear}-{periodEndJob.CollectionPeriod}.");
        }

        private JobType ToJobType(RecordPeriodEndJob periodEndJob)
        {
            if (periodEndJob is RecordPeriodEndStartJob)
                return JobType.PeriodEndStartJob;
            if (periodEndJob is RecordPeriodEndRunJob)
                return JobType.PeriodEndRunJob;
            if (periodEndJob is RecordPeriodEndStopJob)
                return JobType.PeriodEndStopJob;
            throw new InvalidOperationException($"Unhandled period end job type: {periodEndJob.GetType().Name}");
        }

    }
}