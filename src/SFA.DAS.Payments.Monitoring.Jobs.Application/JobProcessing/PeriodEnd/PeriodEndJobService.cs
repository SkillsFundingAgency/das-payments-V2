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
        Task RecordPeriodEndStart(long jobId, short collectionYear, byte collectionPeriod, CancellationToken cancellationToken);
        Task RecordPeriodEndRun(long jobId, short collectionYear, byte collectionPeriod, CancellationToken cancellationToken);
        Task RecordPeriodEndStop(long jobId, short collectionYear, byte collectionPeriod, CancellationToken cancellationToken);
    }
    
    public class PeriodEndJobService : JobService,IPeriodEndJobService
    {
        private readonly IPaymentLogger logger;

        public PeriodEndJobService(IPaymentLogger logger, IJobStorageService jobStorageService, ITelemetry telemetry) : base(logger, jobStorageService, telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task RecordNewJob(JobModel jobDetails, CancellationToken cancellationToken)
        {
            Logger.LogDebug($"Now recording new period end job.  Job Id: {jobDetails.DcJobId}");
            jobDetails.StartTime = DateTimeOffset.Now;
            await RecordNewJob(jobDetails,cancellationToken).ConfigureAwait(false);
        }
        
       
        public async Task RecordPeriodEndStart(long jobId, short collectionYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Sending request to record period end start. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
           
            var jobDetails = new JobModel
            {
                JobType = JobType.PeriodEndStartJob,
                CollectionPeriod =collectionPeriod,
                AcademicYear = collectionYear,
                DcJobId = jobId,
                Status = JobStatus.InProgress,
            };
            await RecordNewJob(jobDetails, cancellationToken).ConfigureAwait(false);
            
            logger.LogInfo($"Sent request to record period end start job. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
        }

        public async Task RecordPeriodEndRun(long jobId, short collectionYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Sending request to record period end run. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
            var jobDetails = new JobModel
            {
                JobType = JobType.PeriodEndRunJob,
                CollectionPeriod =collectionPeriod,
                AcademicYear = collectionYear,
                DcJobId = jobId,
                Status = JobStatus.InProgress,
            };
            await RecordNewJob(jobDetails, cancellationToken).ConfigureAwait(false);
            logger.LogInfo($"Sent request to record period end run job. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
        }

        public async Task RecordPeriodEndStop(long jobId, short collectionYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Sending request to record period end stop. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
            var jobDetails = new JobModel
            {
                JobType = JobType.PeriodEndStopJob,
                CollectionPeriod =collectionPeriod,
                AcademicYear = collectionYear,
                DcJobId = jobId,
                Status = JobStatus.InProgress,
            };
            await RecordNewJob(jobDetails, cancellationToken).ConfigureAwait(false);
            logger.LogInfo($"Sent request to record period end stop job. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
        }
    }
}