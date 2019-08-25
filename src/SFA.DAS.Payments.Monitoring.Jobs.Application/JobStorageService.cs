using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Amqp.Serialization;
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
        public async Task StoreJob(JobModel job, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<List<JobStepModel>> GetJobMessages(List<Guid> messageIds, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task StoreJobMessages(List<JobStepModel> jobMessages, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}