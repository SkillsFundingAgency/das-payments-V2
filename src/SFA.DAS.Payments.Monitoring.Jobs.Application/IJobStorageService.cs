using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface IJobStorageService
    {
        Task<bool> StoreNewJob(JobModel job, CancellationToken cancellationToken);
        Task SaveJobStatus(long jobId, JobStatus jobStatus, DateTimeOffset endTime, CancellationToken cancellationToken);
        Task<JobModel> GetJob(long jobId, CancellationToken cancellationToken);
        Task<List<InProgressMessage>> GetInProgressMessages(long jobId, CancellationToken cancellationToken);
        Task RemoveInProgressMessages(long jobId, List<Guid> messageIdentifiers, CancellationToken cancellationToken);
        Task StoreInProgressMessages(long jobId, List<InProgressMessage> inProgressMessages, CancellationToken cancellationToken);
        Task<List<CompletedMessage>> GetCompletedMessages(long jobId, CancellationToken cancellationToken);
        Task RemoveCompletedMessages(long jobId, List<Guid> completedMessages, CancellationToken cancellationToken);
        Task StoreCompletedMessage(CompletedMessage completedMessage, CancellationToken cancellationToken);
        Task<(bool hasFailedMessages, DateTimeOffset? endTime)> GetJobStatus(long jobId, CancellationToken cancellationToken);
        Task StoreJobStatus(long jobId, bool hasFailedMessages, DateTimeOffset? endTime, CancellationToken cancellationToken);
        Task SaveDataLocksCompletionTime(long jobId, DateTimeOffset endTime, CancellationToken cancellationToken);
        Task<List<long>> GetCurrentEarningJobs(CancellationToken cancellationToken);
        Task<List<long>> GetCurrentPeriodEndJobs(CancellationToken cancellationToken);
        Task StoreDcJobStatus(long jobId, bool succeeded, CancellationToken cancellationToken);
    }
}