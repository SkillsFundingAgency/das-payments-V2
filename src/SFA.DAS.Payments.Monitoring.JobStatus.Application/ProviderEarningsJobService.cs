using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.JobStatus.Application.Data;
using SFA.DAS.Payments.Monitoring.JobStatus.Application.Data.Model;
using SFA.DAS.Payments.Monitoring.JobStatus.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Application
{
    public interface IProviderEarningsJobService
    {
        Task JobStarted(StartedProcessingProviderEarningsEvent startedEvent);
        Task JobStepCompleted(ProcessedPaymentsMessageEvent stepCompleted);
    }

    public class ProviderEarningsJobService: IProviderEarningsJobService
    {
        private readonly IPaymentLogger logger;
        private readonly IJobStatusDataContext dataContext;

        public ProviderEarningsJobService(IPaymentLogger logger, IJobStatusDataContext dataContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task JobStarted(StartedProcessingProviderEarningsEvent startedEvent)
        {
            logger.LogDebug($"Now recording new provider earnings job.  Job Id: {startedEvent.JobId}, Ukprn: {startedEvent.Ukprn}.");
            var job = new JobModel
            {
                Status = Data.Model.JobStatus.InProgress,
                StartTime = startedEvent.StartTime,
                ProviderEarnings = new List<ProviderEarningsJobModel>
                {
                    new ProviderEarningsJobModel
                    {
                        DcJobId = startedEvent.JobId,
                        CollectionPeriod = startedEvent.CollectionPeriod,
                        CollectionYear = startedEvent.CollectionYear,
                        Ukprn = startedEvent.Ukprn,
                        IlrSubmissionTime = startedEvent.IlrSubmissionTime
                    }
                },
                JobEvents = startedEvent.SubEventIds
                    .Select(item => new JobStepModel
                    {
                        MessageId = item.EventId,
                        StartTime = item.StartTime,
                        Status = JobEventStatus.Queued,

                    })
                    .ToList()
            };

            logger.LogVerbose($"Now storing the job in the data context. DC job id: {startedEvent.JobId}, Ukprn: {startedEvent.Ukprn}.");
            await dataContext.SaveNewJob(job);
            logger.LogInfo($"Finished saving the job to the db.  Job id: {job.Id}, DC Job Id: {startedEvent.JobId}, Ukprn: {startedEvent.Ukprn}.");
        }

        public async Task JobStepCompleted(ProcessedPaymentsMessageEvent stepCompleted)
        {
            //var jobId = await dataContext.GetJobIdFromDcJobId(stepCompleted.JobId);
            
        }
    }
}