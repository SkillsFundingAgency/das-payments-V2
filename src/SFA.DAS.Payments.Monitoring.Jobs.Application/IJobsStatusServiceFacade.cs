using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface IJobsStatusServiceFacade
    {
        Task JobStepsCompleted(long jobId);
    }
}