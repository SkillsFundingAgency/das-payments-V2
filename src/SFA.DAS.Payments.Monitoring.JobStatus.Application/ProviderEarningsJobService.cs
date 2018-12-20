using System;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.JobStatus.Application.Data;
using SFA.DAS.Payments.Monitoring.JobStatus.Application.Data.Model;
using SFA.DAS.Payments.Monitoring.JobStatus.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Application
{
    public interface IProviderEarningsJobService
    {
        Task JobStarted(RecordStartedProcessingProviderEarningsJob startedEvent);
        Task JobStepCompleted(RecordJobMessageProcessingStatus stepCompleted);
    }

    public class ProviderEarningsJobService : IProviderEarningsJobService
    {
        private readonly IPaymentLogger logger;
        private readonly IJobStatusDataContext dataContext;

        public ProviderEarningsJobService(IPaymentLogger logger, IJobStatusDataContext dataContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task JobStarted(RecordStartedProcessingProviderEarningsJob startedEvent)
        {
            logger.LogDebug($"Now recording new provider earnings job.  Job Id: {startedEvent.JobId}, Ukprn: {startedEvent.Ukprn}.");
            
            var job = await dataContext.SaveNewProviderEarningsJob((startedEvent.JobId, startedEvent.StartTime,
                startedEvent.CollectionPeriod, startedEvent.CollectionYear, startedEvent.Ukprn,
                startedEvent.IlrSubmissionTime, startedEvent.SubEventIds));

            logger.LogInfo($"Finished saving the job to the db.  Job id: {job.Id}, DC Job Id: {startedEvent.JobId}, Ukprn: {startedEvent.Ukprn}.");
        }

        public async Task JobStepCompleted(RecordJobMessageProcessingStatus stepCompleted)
        {
            logger.LogDebug($"Now recording completion of message processing.  Job Id: {stepCompleted.JobId}, Message id: {stepCompleted.Id}.");
            await dataContext.StoreMessageProcessingStatus((stepCompleted.JobId, stepCompleted.Id,
                stepCompleted.EndTime, stepCompleted.Succeeded ? JobStepStatus.Completed : JobStepStatus.Failed,
                stepCompleted.GeneratedEvents));
            logger.LogDebug($"Recorded completion of message processing.  Job Id: {stepCompleted.JobId}, Message id: {stepCompleted.Id}.");
        }
    }
}