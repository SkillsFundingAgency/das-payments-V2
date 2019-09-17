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
        Task UpdateJob(JobModel job, CancellationToken cancellationToken);
        Task<JobModel> GetJob(long jobId, CancellationToken cancellationToken);
        Task<List<Guid>> GetInProgressMessageIdentifiers(long jobId, CancellationToken cancellationToken);
        Task RemoveInProgressMessageIdentifiers(long jobId, List<Guid> messageIdentifiers, CancellationToken cancellationToken);
        Task StoreInProgressMessageIdentifiers(long jobId, List<Guid> inProgressMessageIdentifiers, CancellationToken cancellationToken);
        Task<List<CompletedMessage>> GetCompletedMessages(long jobId, CancellationToken cancellationToken);
        Task RemoveCompletedMessages(long jobId, List<Guid> completedMessages, CancellationToken cancellationToken);
        Task StoreCompletedMessage(CompletedMessage completedMessage, CancellationToken cancellationToken);
        Task<(JobStepStatus jobStatus, DateTimeOffset? endTime)> GetJobStatus(long jobId, CancellationToken cancellationToken);
        Task StoreJobStatus(long jobId, JobStepStatus jobStatus, DateTimeOffset? endTime, CancellationToken cancellationToken);
    }
}