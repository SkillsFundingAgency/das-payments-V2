using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{
    public interface IPeriodEndJobService
    {
        Task RecordPeriodEndJob(RecordPeriodEndJob periodEndJob, CancellationToken cancellationToken);
    }
    
    public class PeriodEndJobService : JobService,IPeriodEndJobService
    {
        private readonly IPaymentLogger logger;

        public PeriodEndJobService(IPaymentLogger logger, IJobStorageService jobStorageService, ITelemetry telemetry) : base(logger, jobStorageService, telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
       
        public async Task RecordPeriodEndJob(RecordPeriodEndJob periodEndJob, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Sending request to record {periodEndJob.GetType().Name}. Job Id: {periodEndJob.JobId}, collection period: {periodEndJob.CollectionYear}-{periodEndJob.CollectionPeriod}");
            var jobDetails = new JobModel
            {
                JobType = GetJobType(periodEndJob),
                CollectionPeriod =periodEndJob.CollectionPeriod,
                AcademicYear = periodEndJob.CollectionYear,
                DcJobId = periodEndJob.JobId,
                Status = JobStatus.InProgress,
                StartTime = DateTimeOffset.UtcNow
            };

            await RecordNewJob(jobDetails,periodEndJob.GeneratedMessages,  cancellationToken).ConfigureAwait(false);
            logger.LogInfo($"Sent request to record {periodEndJob.GetType().Name}. Job Id: {periodEndJob.JobId}, collection period: {periodEndJob.CollectionYear}-{periodEndJob.CollectionPeriod}");

        }

        private JobType GetJobType(RecordPeriodEndJob periodEndJob)
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