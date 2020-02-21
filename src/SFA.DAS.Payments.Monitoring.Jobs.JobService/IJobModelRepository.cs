using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService
{
    public interface IJobModelRepository
    {
        Task UpsertJob(JobModel job, CancellationToken cancellationToken);
        Task<JobModel> GetJob(long jobId, CancellationToken cancellationToken);
        Task<List<long>> GetJobIdsByQuery(Func<JobModel, bool> query, CancellationToken cancellationToken);
        Task SaveDataLocksCompletionTime(long jobId, DateTimeOffset endTime, CancellationToken cancellationToken);
    }
}