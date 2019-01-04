using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface IJobsStatusServiceFacade
    {
        Task<(bool Finished, DateTimeOffset? endTime)> JobStepsCompleted(long jobId);
    }
}