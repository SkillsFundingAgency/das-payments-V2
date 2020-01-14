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
        Task RecordPeriodEndStart(long jobId, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages, CancellationToken cancellationToken);
        Task RecordPeriodEndRun(long jobId, short collectionYear, byte collectionPeriod,List<GeneratedMessage> generatedMessages, CancellationToken cancellationToken);
        Task RecordPeriodEndStop(long jobId, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages,CancellationToken cancellationToken);

        Task RecordDcJobCompleted(long jobId, bool succeeded, CancellationToken cancellationToken);

    }
    
    public class PeriodEndJobService : JobService,IPeriodEndJobService
    {
        private readonly IPaymentLogger logger;

        public PeriodEndJobService(IPaymentLogger logger, IJobStorageService jobStorageService, ITelemetry telemetry) : base(logger, jobStorageService, telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

       
        public async Task RecordPeriodEndStart(long jobId, short collectionYear, byte collectionPeriod, List<GeneratedMessage> generatedMessages, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Sending request to record period end start. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
           
            var jobDetails = new JobModel
            {
                JobType = JobType.PeriodEndStartJob,
                CollectionPeriod =collectionPeriod,
                AcademicYear = collectionYear,
                DcJobId = jobId,
                Status = JobStatus.InProgress,
                StartTime = DateTimeOffset.UtcNow
            };
            await RecordNewJob(jobDetails, generatedMessages, cancellationToken).ConfigureAwait(false);
            
            logger.LogInfo($"Sent request to record period end start job. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
        }

        public async Task RecordPeriodEndRun(long jobId, short collectionYear, byte collectionPeriod,List<GeneratedMessage> generatedMessages, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Sending request to record period end run. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
            var jobDetails = new JobModel
            {
                JobType = JobType.PeriodEndRunJob,
                CollectionPeriod =collectionPeriod,
                AcademicYear = collectionYear,
                DcJobId = jobId,
                Status = JobStatus.InProgress,
                StartTime = DateTimeOffset.UtcNow
            };
            await RecordNewJob(jobDetails,generatedMessages,  cancellationToken).ConfigureAwait(false);
            logger.LogInfo($"Sent request to record period end run job. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
        }

        public async Task RecordPeriodEndStop(long jobId, short collectionYear, byte collectionPeriod,List<GeneratedMessage> generatedMessages, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Sending request to record period end stop. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
            var jobDetails = new JobModel
            {
                JobType = JobType.PeriodEndStopJob,
                CollectionPeriod =collectionPeriod,
                AcademicYear = collectionYear,
                DcJobId = jobId,
                Status = JobStatus.InProgress,
                StartTime = DateTimeOffset.UtcNow
            };
            await RecordNewJob(jobDetails,generatedMessages,  cancellationToken).ConfigureAwait(false);
            logger.LogInfo($"Sent request to record period end stop job. Job Id: {jobId}, collection period: {collectionYear}-{collectionPeriod}");
        }

        
        public async Task RecordDcJobCompleted(long jobId, bool succeeded, CancellationToken cancellationToken)
        {
            Logger.LogDebug($"Now storing the completion status of the submission job. Id: {jobId}, succeeded: {succeeded}");
            await JobStorageService.StoreDcJobStatus(jobId, succeeded, cancellationToken).ConfigureAwait(false);
            Logger.LogInfo($"Finished storing the completion status of the submission job. Id: {jobId}, succeeded: {succeeded}");
        }

    }
}