using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface IJobStorageService
    {
        Task StoreJob(JobModel job, CancellationToken cancellationToken);
        Task<List<JobStepModel>> GetJobMessages(List<Guid> messageIds, CancellationToken cancellationToken);
        Task StoreJobMessages(List<JobStepModel> jobMessages, CancellationToken cancellationToken);
    }

    public class JobStorageService: IJobStorageService
    {
        private readonly IActorDataCache<JobModel> jobCache;
        private readonly IActorDataCache<JobStepModel> jobMessagesCache;
        private readonly IJobsDataContext dataContext;
        private readonly IPaymentLogger logger;
        public const string JobCacheKey = "job";

        public JobStorageService(IActorDataCache<JobModel> jobCache, IActorDataCache<JobStepModel> jobMessagesCache, IJobsDataContext dataContext, IPaymentLogger logger)
        {
            this.jobCache = jobCache ?? throw new ArgumentNullException(nameof(jobCache));
            this.jobMessagesCache = jobMessagesCache ?? throw new ArgumentNullException(nameof(jobMessagesCache));
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StoreJob(JobModel job, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cachedJob = await jobCache.TryGet(JobCacheKey, cancellationToken).ConfigureAwait(false); 
            if (cachedJob.HasValue)
            {
                logger.LogDebug($"Job has already been stored.");
                return;
            }

            await jobCache.AddOrReplace(JobCacheKey, job, cancellationToken).ConfigureAwait(false); 
            await SaveToDatabase(job, cancellationToken).ConfigureAwait(false); 
        }

        private async Task SaveToDatabase(JobModel job, CancellationToken cancellationToken)
        {
            if (job.Id == 0)
                await dataContext.SaveNewJob(job, cancellationToken).ConfigureAwait(false); 
        }

        public async Task<List<JobStepModel>> GetJobMessages(List<Guid> messageIds, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var jobMessages = new List<JobStepModel>();
            //TODO: must be a more performant way of doing this
            foreach (var messageId in messageIds)
            {
                var cachedJobMessage = await jobMessagesCache.TryGet(messageId.ToString(), cancellationToken).ConfigureAwait(false);
                if (cachedJobMessage.HasValue)
                    jobMessages.Add(cachedJobMessage.Value);
            }

            return jobMessages;
        }

        public async Task StoreJobMessages(List<JobStepModel> jobMessages, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var jobMessage in jobMessages)
            {
                await jobMessagesCache.AddOrReplace(jobMessage.MessageId.ToString(), jobMessage, cancellationToken);
            }
        }
    }
}